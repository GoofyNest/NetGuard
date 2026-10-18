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

        // Guard against re-entrant disconnection
        private int _disconnecting = 0;

        // Per-direction "a receive is pending" flags.
        // [0] = client, [1] = server. 0 = idle (safe to arm), 1 = pending (don't double-arm).
        private readonly int[] _receiving = new int[2];

        // Outbound queues drained after each filter/Landing runs.
        private readonly ConcurrentQueue<Packet> _outToClient = new();
        private readonly ConcurrentQueue<Packet> _outToServer = new();

        // Reusable context object; created in the ctor after _client is set.
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

            //SessionBus.PlayerIp = _client.PlayerInfo.IpInfo.Ip;
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

                // Only arm client receive — the server side will be armed lazily
                // by the handler flow (PacketResultType.DoReceive).
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

        void ISessionTransport.EnqueueClient(Packet p) => _outToClient.Enqueue(p);
        void ISessionTransport.EnqueueServer(Packet p) => _outToServer.Enqueue(p);

        private async Task DrainOutboundAsync()
        {
            bool toClient = false;
            while (_outToClient.TryDequeue(out var p))
            {
                _localSecurity.Send(p);
                toClient = true;
            }
            if (toClient) await Send(false);

            bool toServer = false;
            while (_outToServer.TryDequeue(out var p))
            {
                _remoteSecurity.Send(p);
                toServer = true;
            }
            if (toServer) await Send(true);
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
            }
            else
            {
                _remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
            }

            var receivedPackets = isClient ? _localSecurity.TransferIncoming() : _remoteSecurity.TransferIncoming();

            if (receivedPackets == null)
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

                        // Handler says: swallow this packet, don't forward it,
                        // but make sure the server side is armed for the next read.
                        // DoReceive is idempotent thanks to _receiving[].
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

                // Idempotent arm. If a receive is already pending on this
                // direction, do NOT arm another one.
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

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, new SocketState(socket, buffer));

                await Task.CompletedTask;
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

        async void OnReceive(IAsyncResult iar)
        {
            try
            {
                if (iar.AsyncState is not SocketState state || state.Socket == null)
                {
                    Custom.WriteLine("Error: Socket or state is null.", ConsoleColor.Red);
                    return;
                }

                Socket socket = state.Socket;

                // Clear the "pending" flag for this direction — receive completed.
                if (socket == _clientSocket)
                    Interlocked.Exchange(ref _receiving[RecvClient], 0);
                else if (socket == ModuleSocket)
                    Interlocked.Exchange(ref _receiving[RecvServer], 0);

                int bytesReceived = socket.EndReceive(iar);

                // 0 = remote sent FIN. Disconnect.
                if (bytesReceived == 0)
                {
                    HandleDisconnection();
                    return;
                }

                if (Volatile.Read(ref _disconnecting) == 1)
                    return;

                if (socket == _clientSocket)
                {
                    await HandleReceivedData(bytesReceived, true);

                    if (Volatile.Read(ref _disconnecting) == 0)
                        await DoReceive(true);
                }
                else if (socket == ModuleSocket)
                {
                    await HandleReceivedData(bytesReceived, false);

                    if (Volatile.Read(ref _disconnecting) == 0)
                        await DoReceive(false);
                }
            }
            catch (ObjectDisposedException)
            {
                HandleDisconnection();
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"SocketException in OnReceive: {ex.SocketErrorCode}", ConsoleColor.Yellow);
                HandleDisconnection();
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Exception in OnReceive: {ex}", ConsoleColor.Red);
                HandleDisconnection();
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

            for (int i = 0; i < count; i++)
            {
                try
                {
                    var packet = outgoing[i];
                    await socket.SendAsync(packet.Key.Buffer);

                    if (toHost)
                    {
                        _bytesReceivedFromClient += (ulong)packet.Key.Size;
                        HandleClientFlooding();
                    }
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

            // Close the proxy-to-server socket (handler doesn't know about this one)
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

            // Let AsyncServer.OnClientDisconnect handle the client socket — that's its contract
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