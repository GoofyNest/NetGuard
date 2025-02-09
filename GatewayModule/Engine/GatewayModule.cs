using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GatewayModule.Classes;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.PacketManager.Client;
using GatewayModule.PacketManager.Server;
using GatewayModule.Services;
using SilkroadSecurityAPI;
using Module;
using static GatewayModule.Framework.Opcodes.Client;
using static GatewayModule.Framework.Opcodes.Server;

namespace GatewayModule.Engine
{
    sealed class GatewayModule
    {
        private Socket _clientSocket;
        private AsyncServer.DelClientDisconnect _clientDisconnectHandler;
        private object _lock = new object();

        private Socket _moduleSocket;
        private AsyncServer.E_ServerType _handlerType;

        private byte[] _localBuffer = ArrayPool<byte>.Shared.Rent(8192);
        private byte[] _remoteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        private Security _localSecurity = new Security();
        private Security _remoteSecurity = new Security();

        public static FixedSizeQueue<Packet> _lastPackets = new FixedSizeQueue<Packet>(100);
        private ulong _bytesReceivedFromClient = 0;
        private DateTime _startTime = DateTime.Now;

        private ModuleSettings _moduleSettings = Main._moduleSettings;

        private SessionData _client;

        public GatewayModule(Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect)
        {
            _clientSocket = clientSocket;
            _clientDisconnectHandler = delDisconnect;

            _handlerType = AsyncServer.E_ServerType.GatewayModule;
            _moduleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _client = new SessionData
            {
                ip = ((IPEndPoint)_clientSocket.RemoteEndPoint).Address.ToString()
            };

            Custom.WriteLine($"New connection {_client.ip}", ConsoleColor.Cyan);

            try
            {
                _moduleSocket.Connect(new IPEndPoint(IPAddress.Parse(_moduleSettings.moduleIP), _moduleSettings.modulePort));
                _localSecurity.GenerateSecurity(true, true, true);
                DoReceive(true);
                Send(false);
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Remote host ({_moduleSettings.moduleIP}:{_moduleSettings.modulePort}) is unreachable. Exception: {ex}", ConsoleColor.Red);
            }
        }

        void HandleReceivedDataFromServer(int nReceived)
        {
            _remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
            var ReceivePacketFromServer = _remoteSecurity.TransferIncoming();

            if (ReceivePacketFromServer == null)
                return;

            int count = ReceivePacketFromServer.Count;

            for (int i = 0; i < count; i++)
            {
                Packet packet = ReceivePacketFromServer[i];

                IPacketHandler handler = ServerPacketManager.GetHandler(packet, _client);
                
                if(handler != null)
                {
                    var result = handler.Handle(packet, _client);

                    switch (result.ResultType)
                    {
                        case PacketResultType.Block:
                            Custom.WriteLine($"Prevented [{packet.Opcode}] from being sent from {_client.ip}", ConsoleColor.Red);
                            continue;

                        case PacketResultType.Disconnect:
                            Custom.WriteLine($"Disconnected  {_client.ip} for sending [{packet.Opcode}]", ConsoleColor.Red);
                            HandleDisconnection();
                            continue;

                        case PacketResultType.Ban:
                            Custom.WriteLine($"Not implemented", ConsoleColor.Red);
                            continue;

                        case PacketResultType.SkipSending:
                            Send(result.SendImmediately);
                            continue;
                    }

                    if (result.ModifiedPacket != null)
                    {
                        // Send the modified packet instead of the original
                        _localSecurity.Send(result.ModifiedPacket);
                        Send(result.SendImmediately);
                        continue;
                    }

                }

                //var msg = $"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}";
                ////Custom.WriteLine($"[S->C] {packet.Opcode:X4}", ConsoleColor.DarkMagenta);
                //Custom.WriteLine(msg, ConsoleColor.Yellow);

                //File.AppendAllText($"gateway_{Main.unixTimestamp}_server.txt", "\n" + msg);

                switch (packet.Opcode)
                {
                    case LOGIN_SERVER_HANDSHAKE:
                    case CLIENT_ACCEPT_HANDSHAKE:
                        {
                            Send(true);
                            continue;
                        }

                    case SERVER_GATEWAY_PATCH_RESPONSE:
                        _client.sent_id = 1;
                        break;

                    case SERVER_GATEWAY_SHARD_LIST_RESPONSE:
                        _client.sent_list = 1;
                        break;

                    default:
                        //Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                        //Custom.WriteLine($"[S->C] Unknown packet {packet.Opcode:X4} {packet.GetBytes().Length}", ConsoleColor.Yellow);
                        break;
                }

                _localSecurity.Send(packet);
                Send(false);
            }
        }

        private void HandleReceivedDataFromClient(int nReceived)
        {
            _localSecurity.Recv(_localBuffer, 0, nReceived);

            var receivedPackets = _localSecurity.TransferIncoming();

            if (receivedPackets == null)
                return;

            int count = receivedPackets.Count;

            for (int i = 0; i < count; i++)
            {
                var packet = receivedPackets[i];

                var copyOfPacket = packet;

                IPacketHandler handler = ClientPacketManager.GetHandler(packet, _client);

                if (handler != null)
                {
                    var result = handler.Handle(packet, _client);
                    switch (result.ResultType)
                    {
                        case PacketResultType.Block:
                            {
                                Custom.WriteLine($"Prevented [0x{packet.Opcode:X4}] from being sent from {_client.ip}", ConsoleColor.Red);
                                continue;
                            }

                        case PacketResultType.Disconnect:
                            {
                                Custom.WriteLine($"Disconnected  {_client.ip} for sending [0x{packet.Opcode}]", ConsoleColor.Red);
                                HandleDisconnection();
                                continue;
                            }

                        case PacketResultType.Ban:
                            {
                                Custom.WriteLine($"Not implemented", ConsoleColor.Red);
                                continue;
                            }

                        case PacketResultType.SkipSending:
                            {
                                Custom.WriteLine($"SkipSending [0x{packet.Opcode:X4}]", ConsoleColor.DarkMagenta);
                                Send(result.SendImmediately);
                                continue;
                            }

                        case PacketResultType.DoReceive:
                            {
                                Custom.WriteLine($"DoReceive [0x{packet.Opcode:X4}]", ConsoleColor.DarkMagenta);
                                DoReceive(false);
                                continue;
                            }
                    }

                    if (result.ModifiedPacket != null)
                    {
                        // Send the modified packet instead of the original
                        _lastPackets.Enqueue(copyOfPacket);
                        _remoteSecurity.Send(result.ModifiedPacket);
                        Send(result.SendImmediately);
                        continue;
                    }
                }

                _lastPackets.Enqueue(copyOfPacket);
                _remoteSecurity.Send(packet);
                Send(true);
                continue;
            }
        }

        void DoReceive(bool isClient)
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

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, state);
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
                    await Task.Run(() => HandleReceivedDataFromClient(bytesReceived));
                    DoReceive(true);  // Continue receiving from client
                }
                else if (socket == _moduleSocket)
                {
                    // Server-specific data handling
                    await Task.Run(() => HandleReceivedDataFromServer(bytesReceived));
                    DoReceive(false);  // Continue receiving from server
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Exception in OnReceive: {ex}", ConsoleColor.Red);
                HandleDisconnection();
            }
        }

        void Send(bool toHost)
        {
            lock (_lock)
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
                        socket.Send(packet.Key.Buffer);

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

        private readonly object _socketLock = new object();

        private void DisconnectModuleSocket()
        {
            lock (_socketLock)
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
                    _moduleSocket = null;
                }
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