using Module.Config;
using Module.PacketHandler;
using Module.PacketHandler.Filtering;
using Module.Services;
using SilkroadSecurityAPI;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using AgentClientHandler = Module.PacketHandler.Agent.Client.PacketHandler;
using AgentServerHandler = Module.PacketHandler.Agent.Server.PacketHandler;
using GatewayClientHandler = Module.PacketHandler.Gateway.Client.PacketHandler;
using GatewayServerHandler = Module.PacketHandler.Gateway.Server.PacketHandler;

namespace Module.Networking
{
    sealed class Module : ISessionTransport
    {
        public int ClientId { get; }
        private Socket _clientSocket;
        private readonly AsyncServer.DelClientDisconnect _clientDisconnectHandler;

        private Socket ModuleSocket;
        private readonly ModuleType _moduleType;

        private readonly byte[] _localBuffer = ArrayPool<byte>.Shared.Rent(8192);
        private readonly byte[] _remoteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        private readonly Security _localSecurity = new();
        private readonly Security _remoteSecurity = new();

        private ulong _bytesReceivedFromClient = 0;
        private readonly DateTime _startTime = DateTime.Now;

        private readonly ModuleSettings ModuleSettings = Main.Module;

        private readonly SessionData _client = new();

        // Re-entrancy guard for HandleDisconnection. One canonical place closes
        // the sockets; every other caller just trips this flag and returns.
        private int _disconnecting = 0;

        // Per-direction "a receive is pending" flag.
        // [0] = client, [1] = server. 0 = idle (safe to arm), 1 = pending.
        private readonly int[] _receiving = new int[2];

        // Outbound queues drained after each filter/landing run.
        private readonly ConcurrentQueue<Packet> _outToClient = new();
        private readonly ConcurrentQueue<Packet> _outToServer = new();

        // Fast-path flags: skip the async state machine when nothing is queued.
        private int _hasOutToClient = 0;
        private int _hasOutToServer = 0;

        private readonly SessionContext _ctx;

        private const int RecvClient = 0;
        private const int RecvServer = 1;

        public Module(int clientId, Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect, ModuleType moduleType)
        {
            ClientId = clientId;
            _clientSocket = clientSocket;
            _clientDisconnectHandler = delDisconnect;

            _moduleType = moduleType;
            ModuleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _client.PlayerInfo.IpInfo = new();

            switch (moduleType)
            {
                case ModuleType.GatewayModule:
                    _client.Gateway = new();
                    break;

                case ModuleType.AgentModule:
                    _client.Agent = new();
                    break;
            }

            if (_clientSocket.RemoteEndPoint is IPEndPoint endpoint)
            {
                _client.PlayerInfo.IpInfo.Ip = endpoint.Address.ToString();
            }

            SessionBus.Register(_client);

            FilterRegistry.Loader?.AttachSession(_client);
            FilterRegistry.LiveSessions[ClientId] = _client;
            _ctx = new SessionContext(_client, this);

            Custom.WriteLine($"New connection {_client.PlayerInfo.IpInfo.Ip}", ConsoleColor.Cyan);
        }

        public async Task StartAsync()
        {
            try
            {
                _localSecurity.GenerateSecurity(true, true, true);

                await ModuleSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(ModuleSettings.ModuleIP), ModuleSettings.ModulePort));
                ModuleSocket.NoDelay = true;

                await DoReceive(true);

                await Send(false);
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"Remote host ({ModuleSettings.ModuleIP}:{ModuleSettings.ModulePort}) is unreachable. Exception: {ex}", ConsoleColor.Red);
                HandleDisconnection();
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Exception in Module StartAsync: {ex}", ConsoleColor.Red);
                HandleDisconnection();
            }
        }

        void ISessionTransport.EnqueueClient(Packet p)
        {
            _outToClient.Enqueue(p);
            Volatile.Write(ref _hasOutToClient, 1);
        }

        void ISessionTransport.EnqueueServer(Packet p)
        {
            _outToServer.Enqueue(p);
            Volatile.Write(ref _hasOutToServer, 1);
        }

        private async Task DrainOutboundAsync()
        {
            // Fast path — nothing to drain, no async state machine allocated.
            if (Volatile.Read(ref _hasOutToClient) == 0 && Volatile.Read(ref _hasOutToServer) == 0)
                return;

            if (Volatile.Read(ref _hasOutToClient) == 1)
            {
                Volatile.Write(ref _hasOutToClient, 0);
                bool any = false;
                while (_outToClient.TryDequeue(out var p))
                {
                    _localSecurity.Send(p);
                    any = true;
                }
                if (any) await Send(false);
            }

            if (Volatile.Read(ref _hasOutToServer) == 1)
            {
                Volatile.Write(ref _hasOutToServer, 0);
                bool any = false;
                while (_outToServer.TryDequeue(out var p))
                {
                    _remoteSecurity.Send(p);
                    any = true;
                }
                if (any) await Send(true);
            }
        }

        async Task HandleReceivedData(int nReceived, bool isClient)
        {
            try { await HandleReceivedDataInner(nReceived, isClient); }
            finally { await DrainOutboundAsync(); }
        }

        async Task HandleReceivedDataInner(int nReceived, bool isClient)
        {
            if (Volatile.Read(ref _disconnecting) == 1)
                return;

            if (isClient)
            {
                _localSecurity.Recv(_localBuffer, 0, nReceived);
                _bytesReceivedFromClient += (ulong)nReceived;
                HandleClientFlooding();
            }
            else
            {
                _remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
            }

            var receivedPackets = isClient ? _localSecurity.TransferIncoming() : _remoteSecurity.TransferIncoming();

            if (receivedPackets == null || receivedPackets.Count == 0)
                return;

            int count = receivedPackets.Count;

            for (int i = 0; i < count; i++)
            {
                if (Volatile.Read(ref _disconnecting) == 1)
                    return;

                Packet packet = receivedPackets[i];

                IPacketHandler handler = null!;

                if (_moduleType == ModuleType.GatewayModule)
                {
                    handler = isClient
                        ? GatewayClientHandler.GetHandler(packet, _client, _ctx)
                        : GatewayServerHandler.GetHandler(packet, _client, _ctx);
                }
                else if (_moduleType == ModuleType.AgentModule)
                {
                    handler = isClient
                        ? AgentClientHandler.GetHandler(packet, _client, _ctx)
                        : AgentServerHandler.GetHandler(packet, _client, _ctx);
                }

                if (handler != null)
                {
                    var result = handler.Handle(packet, _client, _ctx);

                    FilterRegistry.Loader?.SyncSession(_client);

                    switch (result.ResultType)
                    {
                        case PacketResultType.Block:
                            Custom.WriteLine($"Prevented [0x{packet.Opcode:X4}] from being sent from {_client.PlayerInfo.IpInfo.Ip}", ConsoleColor.Red);
                            continue;

                        case PacketResultType.Disconnect:
                            Custom.WriteLine($"Disconnected {_client.PlayerInfo.IpInfo.Ip} for sending [0x{packet.Opcode}]", ConsoleColor.Red);
                            HandleDisconnection();
                            return;

                        case PacketResultType.Ban:
                            Custom.WriteLine($"Not implemented", ConsoleColor.Red);
                            continue;

                        case PacketResultType.SkipSending:
                            Custom.WriteLine($"SkipSending [0x{packet.Opcode:X4}]", ConsoleColor.Magenta);
                            await Send(result.SkipSending);
                            continue;

                        case PacketResultType.DoReceive:
                            Custom.WriteLine($"DoReceive [0x{packet.Opcode:X4}]", ConsoleColor.Magenta);
                            await DoReceive(false);
                            continue;
                    }

                    var modifiedCount = result.ModifiedPackets.Count;
                    if (modifiedCount > 0)
                    {
                        for (var d = 0; d < modifiedCount; d++)
                        {
                            var modifiedPacket = result.ModifiedPackets[d];

                            switch (modifiedPacket.SecurityType)
                            {
                                case SecurityType.RemoteSecurity:
                                    _remoteSecurity.Send(modifiedPacket.Packet);
                                    await Send(modifiedPacket.SendImmediately);
                                    break;

                                case SecurityType.LocalSecurity:
                                    _localSecurity.Send(modifiedPacket.Packet);
                                    await Send(modifiedPacket.SendImmediately);
                                    break;

                                case SecurityType.Default:
                                    if (isClient)
                                    {
                                        _remoteSecurity.Send(modifiedPacket.Packet);
                                    }
                                    else
                                    {
                                        _localSecurity.Send(modifiedPacket.Packet);
                                    }
                                    await Send(isClient);
                                    break;
                            }
                        }
                        continue;
                    }
                }

                if (isClient)
                {
                    _remoteSecurity.Send(packet);
                }
                else
                {
                    _localSecurity.Send(packet);
                }

                await Send(isClient);
            }
        }

        async Task DoReceive(bool isClient)
        {
            int idx = isClient ? RecvClient : RecvServer;

            try
            {
                if (Volatile.Read(ref _disconnecting) == 1)
                    return;

                // Idempotent arm: never start a second receive loop for a direction
                // that already has one running.
                if (Interlocked.Exchange(ref _receiving[idx], 1) == 1)
                    return;

                Socket socket = isClient ? _clientSocket : ModuleSocket;
                byte[] buffer = isClient ? _localBuffer : _remoteBuffer;

                if (socket == null || !socket.Connected)
                {
                    Custom.WriteLine($"{(isClient ? "Client" : "Server")} socket is not connected or is null.", ConsoleColor.Yellow);
                    Interlocked.Exchange(ref _receiving[idx], 0);
                    HandleDisconnection();
                    return;
                }

                _ = ReceiveLoopAsync(socket, buffer, isClient);
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"Socket error: {ex.Message}", ConsoleColor.Red);
                Interlocked.Exchange(ref _receiving[idx], 0);
                HandleDisconnection();
            }
            catch (ObjectDisposedException)
            {
                Custom.WriteLine("Attempted to use a disposed socket.", ConsoleColor.Gray);
                Interlocked.Exchange(ref _receiving[idx], 0);
                HandleDisconnection();
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Unexpected error: {ex}", ConsoleColor.Red);
                Interlocked.Exchange(ref _receiving[idx], 0);
                HandleDisconnection();
            }
        }

        async Task ReceiveLoopAsync(Socket socket, byte[] buffer, bool isClient)
        {
            int idx = isClient ? RecvClient : RecvServer;

            try
            {
                while (Volatile.Read(ref _disconnecting) == 0)
                {
                    int bytesReceived;

                    try
                    {
                        bytesReceived = await socket.ReceiveAsync(buffer.AsMemory(), SocketFlags.None);
                    }
                    catch (ObjectDisposedException)
                    {
                        HandleDisconnection();
                        return;
                    }
                    catch (SocketException ex)
                    {
                        Custom.WriteLine($"SocketException in ReceiveLoop: {ex.SocketErrorCode}", ConsoleColor.Yellow);
                        HandleDisconnection();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"Exception in ReceiveLoop: {ex}", ConsoleColor.Red);
                        HandleDisconnection();
                        return;
                    }

                    // 0 = remote sent FIN.
                    if (bytesReceived == 0)
                    {
                        HandleDisconnection();
                        return;
                    }

                    if (Volatile.Read(ref _disconnecting) == 1)
                        return;

                    await HandleReceivedData(bytesReceived, isClient);
                }
            }
            finally
            {
                Interlocked.Exchange(ref _receiving[idx], 0);
            }
        }

        async Task Send(bool toHost)
        {
            if (Volatile.Read(ref _disconnecting) == 1)
                return;

            var security = toHost ? _remoteSecurity : _localSecurity;
            if (security == null)
                return;

            var socket = toHost ? ModuleSocket : _clientSocket;
            if (socket == null || !socket.Connected)
                return;

            var outgoing = security.TransferOutgoing();
            if (outgoing == null)
                return;

            int count = outgoing.Count;

            // One TCP write per Silkroad packet — the wire format the client
            // (and server) actually expect. No coalescing.
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var packet = outgoing[i];
                    await socket.SendAsync(packet.Key.Buffer);
                }
                catch (ObjectDisposedException)
                {
                    HandleDisconnection();
                    return;
                }
                catch (SocketException ex)
                {
                    Custom.WriteLine($"SocketException in Send: {ex.SocketErrorCode}", ConsoleColor.Yellow);
                    HandleDisconnection();
                    return;
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"Exception in Send: {ex}", ConsoleColor.Red);
                    HandleDisconnection();
                    return;
                }
            }
        }

        private void HandleClientFlooding()
        {
            double bytesPerSecond = GetBytesPerSecondFromClient();

            if (bytesPerSecond <= 100000)
                return;

            Custom.WriteLine($"Client({_client.PlayerInfo.IpInfo.Ip}) disconnected for flooding.", ConsoleColor.Yellow);

            HandleDisconnection();
        }

        private double GetBytesPerSecondFromClient()
        {
            var elapsedTime = DateTime.Now - _startTime;
            return elapsedTime.TotalSeconds < 1.0
                ? _bytesReceivedFromClient
                : Math.Round(_bytesReceivedFromClient / elapsedTime.TotalSeconds, 2);
        }

        private void HandleDisconnection()
        {
            if (Interlocked.Exchange(ref _disconnecting, 1) == 1)
                return;

            FilterRegistry.LiveSessions.TryRemove(ClientId, out _);
            SessionBus.Unregister(_client);

            Custom.WriteLine($"Disconnecting client {ClientId} ({_client.PlayerInfo.IpInfo.Ip})", ConsoleColor.Yellow);

            try
            {
                if (ModuleSocket != null)
                {
                    if (ModuleSocket.Connected)
                    {
                        try { ModuleSocket.Shutdown(SocketShutdown.Both); } catch { }
                    }
                    ModuleSocket.Close();
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"HandleDisconnection()::Error closing module socket. Exception: {ex}", ConsoleColor.Red);
            }
            finally
            {
                ModuleSocket = null!;
            }

            try
            {
                _clientDisconnectHandler?.Invoke(ClientId, ref _clientSocket, _moduleType);
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"HandleDisconnection()::Error in disconnect callback. Exception: {ex}", ConsoleColor.Red);
            }
            finally
            {
                _clientSocket = null!;
            }
        }
    }
}