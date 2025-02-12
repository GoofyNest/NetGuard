using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Module.Classes;
using Module.Engine.Classes;
using Module.Framework;
using Module.PacketManager.Agent.Client;
using Module.PacketManager.Agent.Server;
using Module.PacketManager.GatewayModule.Client;
using Module.PacketManager.GatewayModule.Server;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Engine
{
    sealed class Module
    {
        private Socket _clientSocket;
        private AsyncServer.DelClientDisconnect _clientDisconnectHandler;

        private Socket _moduleSocket;
        private ModuleType _handlerType;

        private byte[] _localBuffer = ArrayPool<byte>.Shared.Rent(8192);
        private byte[] _remoteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        private Security _localSecurity = new();
        private Security _remoteSecurity = new();

        public static FixedSizeQueue<Packet> _lastPackets = new(100);
        private ulong _bytesReceivedFromClient = 0;
        private DateTime _startTime = DateTime.Now;

        private ModuleSettings _moduleSettings = Main._module;

        private SessionData _client;

        public Module(Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect, ModuleType _serverType)
        {
            _clientSocket = clientSocket;
            _clientDisconnectHandler = delDisconnect;

            _handlerType = _serverType;
            _moduleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (_clientSocket.RemoteEndPoint is IPEndPoint endpoint)
            {
                _client = new()
                {
                    ip = endpoint.Address.ToString()
                };
            }
            else
                _client = new();

            Custom.WriteLine($"New connection {_client.ip}", ConsoleColor.Cyan);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Connect to the remote host asynchronously
                await _moduleSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(_moduleSettings.moduleIP), _moduleSettings.modulePort));

                // Generate security information
                _localSecurity.GenerateSecurity(true, true, true);

                // Begin receiving data
                await DoReceive(true);

                // Start sending data asynchronously
                await Send(false);
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"Remote host ({_moduleSettings.moduleIP}:{_moduleSettings.modulePort}) is unreachable. Exception: {ex}", ConsoleColor.Red);
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Exception in Module StartAsync: {ex}", ConsoleColor.Red);
            }
        }

        async Task HandleReceivedData(int nReceived, bool isClient)
        {
            // Receive data based on whether it's from the client or server
            if (isClient)
            {
                _localSecurity.Recv(_localBuffer, 0, nReceived);
            }
            else
            {
                _remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
            }

            // Transfer the incoming packets
            var receivedPackets = isClient ? _localSecurity.TransferIncoming() : _remoteSecurity.TransferIncoming();

            if (receivedPackets == null)
                return;

            int count = receivedPackets.Count;

            for (int i = 0; i < count; i++)
            {
                Packet packet = receivedPackets[i];
                var copyOfPacket = packet;

                // Get the appropriate packet handler

                IPacketHandler handler = null!;

                if (_handlerType == ModuleType.GatewayModule)
                {
                    handler = isClient ? GatewayClientPacketManager.GetHandler(packet, _client) : GatewayServerPacketManager.GetHandler(packet, _client);
                }
                else if(_handlerType == ModuleType.AgentModule)
                {
                    handler = isClient ? AgentClientPacketManager.GetHandler(packet, _client) : AgentServerPacketManager.GetHandler(packet, _client);
                }

                if (handler != null)
                {
                    var result = handler.Handle(packet, _client);
                    switch (result.ResultType)
                    {
                        case PacketResultType.Block:
                            Custom.WriteLine($"Prevented [0x{packet.Opcode:X4}] from being sent from {_client.ip}", ConsoleColor.Red);
                            continue;
                
                        case PacketResultType.Disconnect:
                            Custom.WriteLine($"Disconnected  {_client.ip} for sending [0x{packet.Opcode}]", ConsoleColor.Red);
                            HandleDisconnection();
                            continue;
                
                        case PacketResultType.Ban:
                            Custom.WriteLine($"Not implemented", ConsoleColor.Red);
                            continue;
                
                        case PacketResultType.SkipSending:
                            Custom.WriteLine($"SkipSending [0x{packet.Opcode:X4}]", ConsoleColor.DarkMagenta);
                            await Send(result.SendImmediately);
                            continue;
                
                        case PacketResultType.DoReceive:
                            Custom.WriteLine($"DoReceive [0x{packet.Opcode:X4}]", ConsoleColor.DarkMagenta);
                            await DoReceive(false);
                            continue;
                    }
                
                    if (result.ModifiedPacket != null)
                    {
                        switch(result.securityType)
                        {
                            case SecurityType.RemoteSecurity:
                                {
                                    _remoteSecurity.Send(result.ModifiedPacket);
                                    await Send(result.SendImmediately);
                                }
                                break;

                            case SecurityType.LocalSecurity:
                                {
                                    _localSecurity.Send(result.ModifiedPacket);
                                    await Send(result.SendImmediately);
                                }
                                break;

                            case SecurityType.Default:
                                {
                                    if (isClient)
                                    {
                                        _lastPackets.Enqueue(copyOfPacket);
                                        _remoteSecurity.Send(result.ModifiedPacket);
                                    }
                                    else
                                    {
                                        _localSecurity.Send(result.ModifiedPacket);
                                    }
                                    await Send(result.SendImmediately);
                                }
                                break;
                        }
                        continue;
                    }
                }

                // Enqueue and send the original packet
                if (isClient)
                {
                    _lastPackets.Enqueue(copyOfPacket);
                    _remoteSecurity.Send(packet);
                }
                else
                {
                    _localSecurity.Send(packet);
                }

                await Send(isClient); // Use the isClient flag to determine the action
            }
        }

        async Task DoReceive(bool isClient)
        {
            try
            {
                Socket socket = isClient ? _clientSocket : _moduleSocket;
                byte[] buffer = isClient ? _localBuffer : _remoteBuffer;

                if (socket == null || !socket.Connected)
                {
                    Custom.WriteLine($"{(isClient ? "Client" : "Server")} socket is not connected or is null.", ConsoleColor.Yellow);
                    return;
                }

                // Create state object and pass it in the BeginReceive
                var state = new SocketState(socket, buffer);

                // Initiate BeginReceive, this does not block, just sets up the receive operation
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, new SocketState(socket, buffer));

                await Task.CompletedTask;
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"Socket error: {ex.Message}", ConsoleColor.Red);
            }
            catch (ObjectDisposedException)
            {
                Custom.WriteLine("Attempted to use a disposed socket.", ConsoleColor.Gray);
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Unexpected error: {ex}", ConsoleColor.Red);
            }
        }

        async void OnReceive(IAsyncResult iar)
        {
            try
            {
                // Retrieve the state object from AsyncState
                var state = iar.AsyncState as SocketState;
                if (state == null || state.Socket == null)
                {
                    Custom.WriteLine("Error: Socket or state is null.", ConsoleColor.Red);
                    return;
                }

                Socket socket = state.Socket;

                int bytesReceived = socket.EndReceive(iar);

                // If no data received, handle disconnect logic
                if (bytesReceived == 0)
                {
                    return;
                }

                // Handle the received data based on the socket type
                if (socket == _clientSocket)
                {
                    // Client-specific data handling
                    await Task.Run(() => HandleReceivedData(bytesReceived, true));
                    await DoReceive(true);  // Continue receiving from client
                }
                else if (socket == _moduleSocket)
                {
                    // Server-specific data handling
                    await Task.Run(() => HandleReceivedData(bytesReceived, false));

                    await DoReceive(false);  // Continue receiving from server
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Exception in OnReceive: {ex}", ConsoleColor.Red);
                HandleDisconnection();
            }
        }

        async Task Send(bool toHost)
        {
            // Determine the security and socket to use based on 'toHost'
            var security = toHost ? _remoteSecurity : _localSecurity;
            if (security == null)
                return;  // Early return if security is null

            var socket = toHost ? _moduleSocket : _clientSocket;
            if (socket == null)
                return;  // Early return if socket is null

            // Get outgoing packets
            var outgoing = security.TransferOutgoing();
            if (outgoing == null)
                return;  // Early return if no outgoing packets

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
                catch (Exception ex)
                {
                    Custom.WriteLine($"Exception in Send: {ex}", ConsoleColor.Red);
                }
            }
        }

        private void HandleClientFlooding()
        {
            double bytesPerSecond = GetBytesPerSecondFromClient();

            if (bytesPerSecond <= 1000)
                return;

            Custom.WriteLine($"Client({_client.ip}) disconnected for flooding.", ConsoleColor.Yellow);

            HandleDisconnection();
        }

        private double GetBytesPerSecondFromClient()
        {
            var elapsedTime = DateTime.Now - _startTime;
            return elapsedTime.TotalSeconds < 1.0 ? _bytesReceivedFromClient : Math.Round(_bytesReceivedFromClient / elapsedTime.TotalSeconds, 2);
        }

        private void DisconnectModuleSocket()
        {
            if (_moduleSocket == null) return;

            try
            {
                if (_moduleSocket.Connected)
                {
                    _moduleSocket.Shutdown(SocketShutdown.Both); // Graceful shutdown
                }
                _moduleSocket.Close();
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"Socket error during disconnect: {ex.Message}", ConsoleColor.Red);
            }
            catch (ObjectDisposedException)
            {
                Custom.WriteLine("Socket was already disposed.", ConsoleColor.Gray);
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Unexpected error during socket disconnect: {ex}", ConsoleColor.Red);
            }
            finally
            {
                _moduleSocket = null!;
            }
        }
        
        private void HandleDisconnection()
        {
            DisconnectModuleSocket();

            // Notify other parts of the system, if delegate is set
            _clientDisconnectHandler?.Invoke(ref _clientSocket, _handlerType);
        }
    }
}