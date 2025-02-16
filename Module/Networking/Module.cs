using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Module.Config;
using Module.PacketHandler;
using Module.Services;
using SilkroadSecurityAPI;
using GatewayServerHandler = Module.PacketHandler.Gateway.Server.PacketHandler;
using GatewayClientHandler = Module.PacketHandler.Gateway.Client.PacketHandler;
using AgentServerHandler = Module.PacketHandler.Agent.Server.PacketHandler;
using AgentClientHandler = Module.PacketHandler.Agent.Client.PacketHandler;

namespace Module.Networking
{
    sealed class Module
    {
        public int ClientId { get; }  // Store client ID
        private Socket _clientSocket;
        private readonly AsyncServer.DelClientDisconnect _clientDisconnectHandler;

        private Socket ModuleSocket;
        private readonly ModuleType _handlerType;

        private readonly byte[] _localBuffer = ArrayPool<byte>.Shared.Rent(8192);
        private readonly byte[] _remoteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        private readonly Security _localSecurity = new();
        private readonly Security _remoteSecurity = new();

        public static FixedSizeQueue<Packet> _lastPackets = new(100);
        private ulong _bytesReceivedFromClient = 0;
        private readonly DateTime _startTime = DateTime.Now;

        private readonly ModuleSettings ModuleSettings = Main.Module;

        private readonly SessionData _client = new();

        public Module(int clientId, Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect, ModuleType _serverType)
        {
            ClientId = clientId;
            _clientSocket = clientSocket;
            _clientDisconnectHandler = delDisconnect;

            _handlerType = _serverType;
            ModuleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _client.PlayerInfo = new()
            {
                IpInfo = new()
            };

            switch (_serverType)
            {
                case ModuleType.GatewayModule:
                    {
                        _client.Gateway = new();
                    }
                    break;

                case ModuleType.AgentModule:
                    {
                        _client.Agent = new();
                    }
                    break;
            }    

            if (_clientSocket.RemoteEndPoint is IPEndPoint endpoint)
            {
                _client.PlayerInfo.IpInfo.Ip = endpoint.Address.ToString();
            }

            Custom.WriteLine($"New connection {_client.PlayerInfo.IpInfo.Ip}", ConsoleColor.Cyan);
        }

        public async Task StartAsync()
        {
            try
            {
                // Generate security information
                _localSecurity.GenerateSecurity(true, true, true);

                // Connect to the remote host asynchronously
                await ModuleSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(ModuleSettings.ModuleIP), ModuleSettings.ModulePort));

                // Begin receiving data
                await DoReceive(true);

                // Start sending data asynchronously
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
                //var copyOfPacket = packet;

                // Get the appropriate packet handler

                IPacketHandler handler = null!;

                if (_handlerType == ModuleType.GatewayModule)
                {
                    handler = isClient ? GatewayClientHandler.GetHandler(packet, _client) : GatewayServerHandler.GetHandler(packet, _client);
                }
                else if(_handlerType == ModuleType.AgentModule)
                {
                    handler = isClient ? AgentClientHandler.GetHandler(packet, _client) : AgentServerHandler.GetHandler(packet, _client);
                }

                if (handler != null)
                {
                    var result = handler.Handle(packet, _client);
                    switch (result.ResultType)
                    {
                        case PacketResultType.Block:
                            Custom.WriteLine($"Prevented [0x{packet.Opcode:X4}] from being sent from {_client.PlayerInfo.IpInfo.Ip}", ConsoleColor.Red);
                            continue;
                
                        case PacketResultType.Disconnect:
                            Custom.WriteLine($"Disconnected  {_client.PlayerInfo.IpInfo.Ip} for sending [0x{packet.Opcode}]", ConsoleColor.Red);
                            HandleDisconnection();
                            continue;
                
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
                    if(modifiedCount > 0)
                    {
                        for (var d = 0; d < modifiedCount; d++)
                        {
                            var modifiedPacket = result.ModifiedPackets[d];

                            switch (modifiedPacket.SecurityType)
                            {
                                case SecurityType.RemoteSecurity:
                                    {
                                        _remoteSecurity.Send(modifiedPacket.Packet);
                                        await Send(modifiedPacket.SendImmediately);
                                    }
                                    break;

                                case SecurityType.LocalSecurity:
                                    {
                                        _localSecurity.Send(modifiedPacket.Packet);
                                        await Send(modifiedPacket.SendImmediately);
                                    }
                                    break;

                                case SecurityType.Default:
                                    {
                                        if (isClient)
                                        {
                                            //_lastPackets.Enqueue(copyOfPacket);
                                            _remoteSecurity.Send(modifiedPacket.Packet);
                                        }
                                        else
                                        {
                                            _localSecurity.Send(modifiedPacket.Packet);
                                        }
                                        await Send(isClient);
                                    }
                                    break;
                            }
                        }
                        continue;
                    }
                }

                // Enqueue and send the original packet
                if (isClient)
                {
                    //_lastPackets.Enqueue(copyOfPacket);
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
                Socket socket = isClient ? _clientSocket : ModuleSocket;
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
                HandleDisconnection();
            }
            catch (ObjectDisposedException)
            {
                Custom.WriteLine("Attempted to use a disposed socket.", ConsoleColor.Gray);
                HandleDisconnection();
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Unexpected error: {ex}", ConsoleColor.Red);
                HandleDisconnection();
            }
        }

        async void OnReceive(IAsyncResult iar)
        {
            try
            {
                // Retrieve the state object from AsyncState
                if (iar.AsyncState is not SocketState state || state.Socket == null)
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
                else if (socket == ModuleSocket)
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

            var socket = toHost ? ModuleSocket : _clientSocket;
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

            Custom.WriteLine($"Client({_client.PlayerInfo.IpInfo.Ip}) disconnected for flooding.", ConsoleColor.Yellow);

            HandleDisconnection();
        }

        private double GetBytesPerSecondFromClient()
        {
            var elapsedTime = DateTime.Now - _startTime;
            return elapsedTime.TotalSeconds < 1.0 ? _bytesReceivedFromClient : Math.Round(_bytesReceivedFromClient / elapsedTime.TotalSeconds, 2);
        }

        private void HandleDisconnection()
        {
            try
            {
                if (ModuleSocket != null)
                {
                    if (ModuleSocket.Connected)
                    {
                        ModuleSocket.Shutdown(SocketShutdown.Both);
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

            // Notify other parts of the system, if delegate is set
            _clientDisconnectHandler?.Invoke(ClientId, ref _clientSocket, _handlerType);
        }
    }
}