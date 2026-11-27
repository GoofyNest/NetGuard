using Module.Config;
using Module.PacketHandler;
using Module.PacketHandler.Filtering;
using Module.Services;
using SilkroadSecurityAPI;
using System.Buffers;
using System.Diagnostics;
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

        private readonly ByteRateLimiter _clientRate = new(100_000, 200_000);
        private readonly SemaphoreSlim _processingGate = new(1, 1);
        private readonly OrderedSocketSender?[] _senders = new OrderedSocketSender?[2];
        private Timer? _handshakeTimer;
        private Timer? _loginTimer;
        private int _loginCompleted;
        private readonly object _receiveLifecycle = new();
        private readonly int[] _returnedBuffers = new int[2];

        private readonly ModuleSettings ModuleSettings = Main.Module;

        private readonly SessionData _client = new();

        // Re-entrancy guard for HandleDisconnection. One canonical place closes
        // the sockets; every other caller just trips this flag and returns.
        private int _disconnecting = 0;

        // Per-direction "a receive is pending" flag.
        // [0] = client, [1] = server. 0 = idle (safe to arm), 1 = pending.
        private readonly int[] _receiving = new int[2];
        private readonly Task[] _receiveTasks = [Task.CompletedTask, Task.CompletedTask];
        private readonly TaskCompletionSource _disconnected = new(TaskCreationOptions.RunContinuationsAsynchronously);

        // Outbound queues drained after each filter/landing run.
        private readonly BoundedPacketQueue _outToClient = new();
        private readonly BoundedPacketQueue _outToServer = new();

        // Fast-path flags: skip the async state machine when nothing is queued.
        private int _hasOutToClient = 0;
        private int _hasOutToServer = 0;

        private readonly SessionContext _ctx;
        private readonly ScriptedPacketSequence _scriptedPackets;

        private const int RecvClient = 0;
        private const int RecvServer = 1;

        public Module(int clientId, Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect, ModuleType moduleType)
        {
            ClientId = clientId;
            _clientSocket = clientSocket;
            _clientDisconnectHandler = delDisconnect;

            _moduleType = moduleType;
            try
            {
                ModuleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _ctx = new SessionContext(_client, this);
                _scriptedPackets = new ScriptedPacketSequence(SendScriptPacketAsync,
                    message => Custom.WriteLine($"[Script] client={ClientId} {message}", ConsoleColor.Yellow));
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

                FilterRegistry.ExecutionGate.EnterReadLock();
                try
                {
                    FilterRegistry.Loader?.AttachSession(_client);
                    FilterRegistry.LiveSessions[ClientId] = _client;
                    SessionBus.Register(_client);
                    DesktopDiagnostics.Current?.Register(ClientId, (packet, toServer, token) => SendScriptPacketAsync(packet, toServer, () => { }, token));
                }
                finally { FilterRegistry.ExecutionGate.ExitReadLock(); }
                Custom.WriteLine($"New connection {_client.PlayerInfo.IpInfo.Ip}", ConsoleColor.Cyan);
            }
            catch
            {
                _scriptedPackets?.Stop();
                ModuleSocket?.Dispose();
                SessionBus.Unregister(_client);
                DesktopDiagnostics.Current?.Unregister(ClientId);
                ((ICollection<KeyValuePair<int, SessionData>>)FilterRegistry.LiveSessions)
                    .Remove(new(ClientId, _client));
                ReturnBuffer(0);
                ReturnBuffer(1);
                throw;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ModuleSettings.ValidateLimits();
                _handshakeTimer = new Timer(_ => ExpireConnection("handshake"), null,
                    TimeSpan.FromSeconds(ModuleSettings.HandshakeTimeoutSeconds), Timeout.InfiniteTimeSpan);
                _loginTimer = new Timer(_ => ExpireConnection("login"), null,
                    TimeSpan.FromSeconds(ModuleSettings.LoginTimeoutSeconds), Timeout.InfiniteTimeSpan);
                _localSecurity.GenerateSecurity(true, true, true);

                using var connectDeadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                connectDeadline.CancelAfter(TimeSpan.FromSeconds(15));
                await ModuleSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(ModuleSettings.ModuleIP), ModuleSettings.ModulePort), connectDeadline.Token);
                ModuleSocket.NoDelay = true;

                await Send(false);
                await DoReceive(true);
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
            if (!_outToClient.TryEnqueue(p)) throw new InvalidOperationException("Client packet queue limit exceeded.");
            Volatile.Write(ref _hasOutToClient, 1);
        }

        void ISessionTransport.EnqueueServer(Packet p)
        {
            if (!_outToServer.TryEnqueue(p)) throw new InvalidOperationException("Server packet queue limit exceeded.");
            Volatile.Write(ref _hasOutToServer, 1);
        }

        void ISessionTransport.MarkLoginCompleted()
        {
            Volatile.Write(ref _loginCompleted, 1);
            Interlocked.Exchange(ref _loginTimer, null)?.Dispose();
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
            long waiting = Stopwatch.GetTimestamp();
            await _processingGate.WaitAsync();
            NetworkMetrics.Gate(waiting);
            try
            {
                await HandleReceivedDataInner(nReceived, isClient);
                await Send(true);
                await Send(false);
                await DrainOutboundAsync();
            }
            finally { _processingGate.Release(); }
        }

        async Task HandleReceivedDataInner(int nReceived, bool isClient)
        {
            if (Volatile.Read(ref _disconnecting) == 1)
                return;

            if (isClient)
            {
                if (!_clientRate.TryConsume(nReceived))
                {
                    Custom.WriteLine($"Client({_client.PlayerInfo.IpInfo.Ip}) disconnected for flooding.", ConsoleColor.Yellow);
                    HandleDisconnection("client_byte_limit");
                    return;
                }
                _localSecurity.Recv(_localBuffer, 0, nReceived);
            }
            else
            {
                _remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
            }

            var receivedPackets = isClient ? _localSecurity.TransferIncoming() : _remoteSecurity.TransferIncoming();
            if (_localSecurity.HandshakeAccepted && _remoteSecurity.HandshakeAccepted)
                Interlocked.Exchange(ref _handshakeTimer, null)?.Dispose();

            if (receivedPackets == null || receivedPackets.Count == 0)
                return;

            int count = receivedPackets.Count;

            for (int i = 0; i < count; i++)
            {
                if (Volatile.Read(ref _disconnecting) == 1)
                    return;

                Packet packet = receivedPackets[i];
                NetworkMetrics.Packet();
                DesktopDiagnostics.Current?.Capture(packet, _client.PlayerInfo.IpInfo.Ip, ClientId, isClient);
                if (!isClient) _scriptedPackets.ObserveServerPacket(packet.Opcode);

                var result = DispatchPacket(packet, isClient);
                if (result != null)
                {
                    switch (result.ResultType)
                    {
                        case PacketResultType.Block:
                            Custom.WriteLine($"Prevented [0x{packet.Opcode:X4}] from being sent from {_client.PlayerInfo.IpInfo.Ip}", ConsoleColor.Red);
                            continue;

                        case PacketResultType.Disconnect:
                            Custom.WriteLine($"Disconnected {_client.PlayerInfo.IpInfo.Ip} for sending [0x{packet.Opcode}]", ConsoleColor.Red);
                            HandleDisconnection("filter");
                            return;

                        case PacketResultType.Ban:
                            Custom.WriteLine("Ban requested: disconnecting; persistent bans are not configured.", ConsoleColor.Red);
                            HandleDisconnection("filter");
                            return;

                        case PacketResultType.SkipSending:
                            Custom.WriteLine($"SkipSending [0x{packet.Opcode:X4}]", ConsoleColor.Magenta);
                            await Send(result.SkipSending);
                            continue;

                        case PacketResultType.DoReceive:
                            Custom.WriteLine($"DoReceive [0x{packet.Opcode:X4}]", ConsoleColor.Magenta);
                            await DoReceive(false);
                            continue;
                    }

                }

                // Recv has already consumed the handshake and queued its reply on
                // the receiving Security instance. Never forward it to the other
                // connection, even if an opcode filter is absent or passes it on.
                // Explicit filter actions above (including SkipSending) still win.
                if (packet.Opcode is 0x5000 or 0x9000)
                {
                    await Send(!isClient);
                    continue;
                }

                if (result != null)
                {
                    var modifiedCount = result.ModifiedPackets.Count;
                    if (modifiedCount > 0)
                    {
                        if (result.ModifiedPackets.Any(p => p.ServerReply.HasValue))
                        {
                            _scriptedPackets.Enqueue(result.ModifiedPackets, isClient);
                            continue;
                        }
                        for (var d = 0; d < modifiedCount; d++)
                        {
                            var modifiedPacket = result.ModifiedPackets[d];

                            switch (modifiedPacket.SecurityType)
                            {
                                case SecurityType.RemoteSecurity:
                                    _remoteSecurity.Send(modifiedPacket.Packet);
                                    await Send(true);
                                    break;

                                case SecurityType.LocalSecurity:
                                    _localSecurity.Send(modifiedPacket.Packet);
                                    await Send(false);
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

                // Flush bounded groups already received together, without a timer
                // delay. The final group is flushed before releasing the gate.
                if ((i + 1) % 32 == 0) await Send(isClient);
            }
        }

        private async Task SendScriptPacketAsync(Packet packet, bool toServer, Action beforeSend, CancellationToken token)
        {
            Task written;
            await _processingGate.WaitAsync(token);
            try
            {
                token.ThrowIfCancellationRequested();
                if (Volatile.Read(ref _disconnecting) != 0) throw new OperationCanceledException(token);
                if (!(toServer ? _remoteSecurity : _localSecurity).HandshakeAccepted)
                    throw new InvalidOperationException("Scripted packets require a completed handshake.");
                beforeSend();
                (toServer ? _remoteSecurity : _localSecurity).Send(packet);
                await Send(toServer);
                if (Volatile.Read(ref _disconnecting) != 0) throw new OperationCanceledException(token);
                written = (_senders[toServer ? 1 : 0]
                    ?? throw new IOException("No socket writer available.")).FlushAsync();
            }
            finally { _processingGate.Release(); }
            await written.WaitAsync(token);
        }

        private PacketHandlingResult? DispatchPacket(Packet packet, bool isClient)
        {
            FilterRegistry.ExecutionGate.EnterReadLock();
            try
            {
                lock (_client.StateSync)
                {
                    if (!_client.FilterStateValid)
                        return new PacketHandlingResult { ResultType = PacketResultType.Disconnect };
                    long started = Stopwatch.GetTimestamp();
                    try
                    {
                        IPacketHandler? handler = _moduleType switch
                        {
                            ModuleType.GatewayModule => isClient
                                ? GatewayClientHandler.GetHandler(packet, _client, _ctx)
                                : GatewayServerHandler.GetHandler(packet, _client, _ctx),
                            ModuleType.AgentModule => isClient
                                ? AgentClientHandler.GetHandler(packet, _client, _ctx)
                                : AgentServerHandler.GetHandler(packet, _client, _ctx),
                            _ => null
                        };
                        var result = handler?.Handle(packet, _client, _ctx);
                        FilterRegistry.Loader?.SyncSession(_client);
                        return result;
                    }
                    finally { NetworkMetrics.Filter(started); }
                }
            }
            finally { FilterRegistry.ExecutionGate.ExitReadLock(); }
        }

        Task DoReceive(bool isClient)
        {
            lock (_receiveLifecycle)
            {
                ArmReceive(isClient);
            }
            return Task.CompletedTask;
        }

        void ArmReceive(bool isClient)
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

                _receiveTasks[idx] = ReceiveLoopAsync(socket, buffer, isClient);
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
                        var security = isClient ? _localSecurity : _remoteSecurity;
                        security.CheckReceiveTimeout();
                        if (security.MassiveTimeRemaining is { } remaining)
                        {
                            using var assemblyDeadline = new CancellationTokenSource(remaining);
                            bytesReceived = await socket.ReceiveAsync(buffer.AsMemory(), SocketFlags.None, assemblyDeadline.Token);
                        }
                        else bytesReceived = await socket.ReceiveAsync(buffer.AsMemory(), SocketFlags.None);
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
                        // A clean FIN must not discard frames accepted just
                        // before it. Freeze producers and drain both writers.
                        await _processingGate.WaitAsync();
                        try
                        {
                            try
                            {
                                await OrderedSocketSender.DrainAsync(_senders.OfType<OrderedSocketSender>(),
                                    TimeSpan.FromSeconds(ModuleSettings.DrainTimeoutSeconds));
                            }
                            catch (TimeoutException)
                            {
                                Custom.WriteLine($"Client {ClientId}: shutdown drain deadline exceeded.", ConsoleColor.Yellow);
                            }
                            finally { HandleDisconnection("peer_closed"); }
                        }
                        finally { _processingGate.Release(); }
                        return;
                    }

                    if (Volatile.Read(ref _disconnecting) == 1)
                        return;

                    NetworkMetrics.Received(bytesReceived);
                    await HandleReceivedData(bytesReceived, isClient);
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Packet processing failed for client {ClientId}: {ex}", ConsoleColor.Red);
                HandleDisconnection("packet_processing_error");
            }
            finally
            {
                lock (_receiveLifecycle)
                {
                    Interlocked.Exchange(ref _receiving[idx], 0);
                    if (Volatile.Read(ref _disconnecting) != 0) ReturnBuffer(idx);
                }
            }
        }

        private static List<KeyValuePair<TransferBuffer, Packet>> FormatOutgoing(Security security)
        {
            long started = Stopwatch.GetTimestamp();
            try { return security.TransferOutgoing(32, OrderedSocketSender.BatchBytes); }
            finally { NetworkMetrics.Format(started); }
        }

        Task Send(bool toHost)
        {
            if (Volatile.Read(ref _disconnecting) == 1) return Task.CompletedTask;
            var security = toHost ? _remoteSecurity : _localSecurity;
            var socket = toHost ? ModuleSocket : _clientSocket;
            if (socket == null || !socket.Connected) return Task.CompletedTask;

            var outgoing = FormatOutgoing(security);
            if (outgoing.Count == 0) return Task.CompletedTask;
            int direction = toHost ? 1 : 0;
            var sender = _senders[direction] ??= new OrderedSocketSender(socket, ex =>
            {
                Custom.WriteLine($"Socket writer failed for client {ClientId}: {ex.Message}", ConsoleColor.Red);
                HandleDisconnection("socket_write_error");
            });
            if (Volatile.Read(ref _disconnecting) != 0)
            {
                sender.Stop();
                return Task.CompletedTask;
            }
            do
            {
                foreach (var frame in outgoing)
                {
                    var bytes = frame.Key.Buffer.AsMemory(frame.Key.Offset, frame.Key.Size);
                    while (!bytes.IsEmpty)
                    {
                        int size = Math.Min(bytes.Length, OrderedSocketSender.BatchBytes);
                        if (!sender.TryEnqueue(bytes[..size]))
                        {
                            Custom.WriteLine($"Client {ClientId} disconnected: outbound queue limit exceeded.", ConsoleColor.Yellow);
                            HandleDisconnection("socket_queue_limit");
                            return Task.CompletedTask;
                        }
                        bytes = bytes[size..];
                    }
                }
                if (Volatile.Read(ref _disconnecting) != 0) return Task.CompletedTask;
                outgoing = FormatOutgoing(security);
            } while (outgoing.Count != 0);
            return Task.CompletedTask;
        }
        internal void Stop() => HandleDisconnection("stopped");

        // Called after StartAsync has completed, so no startup work can create
        // another timer or writer after the completion snapshots below.
        internal async Task WaitForShutdownAsync()
        {
            await _disconnected.Task;
            Task[] receives;
            lock (_receiveLifecycle) receives = [.. _receiveTasks];
            await Task.WhenAll(receives);
            await _scriptedPackets.Completion;
            await Task.WhenAll(_senders.OfType<OrderedSocketSender>().Select(s => s.Completion));
        }

        private void ExpireConnection(string phase)
        {
            if (Volatile.Read(ref _disconnecting) != 0) return;
            if (phase == "handshake" && _localSecurity.HandshakeAccepted && _remoteSecurity.HandshakeAccepted) return;
            if (phase == "login" && Volatile.Read(ref _loginCompleted) != 0) return;
            Custom.WriteLine($"Client {ClientId}: {phase} deadline exceeded.", ConsoleColor.Yellow);
            HandleDisconnection(phase + "_timeout");
        }

        private void ReturnBuffer(int direction)
        {
            if (Interlocked.Exchange(ref _returnedBuffers[direction], 1) == 0)
                ArrayPool<byte>.Shared.Return(direction == RecvClient ? _localBuffer : _remoteBuffer);
        }

        private void HandleDisconnection(string reason = "transport")
        {
            if (Interlocked.Exchange(ref _disconnecting, 1) == 1)
                return;

            NetworkMetrics.Disconnected(reason);
            foreach (var sender in _senders) sender?.Stop();
            _scriptedPackets.Stop();
            Interlocked.Exchange(ref _handshakeTimer, null)?.Dispose();
            Interlocked.Exchange(ref _loginTimer, null)?.Dispose();

            FilterRegistry.LiveSessions.TryRemove(ClientId, out _);
            SessionBus.Unregister(_client);
            DesktopDiagnostics.Current?.Unregister(ClientId);

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
                _clientSocket?.Dispose();
                _clientSocket = null!;
            }
            lock (_receiveLifecycle)
            {
                for (int i = 0; i < 2; i++)
                    if (Volatile.Read(ref _receiving[i]) == 0) ReturnBuffer(i);
            }
            _disconnected.TrySetResult();
        }
    }
}
