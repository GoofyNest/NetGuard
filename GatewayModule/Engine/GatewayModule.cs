using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GatewayModule.Classes;
using Module;
using NetGuard.Services;
using SilkroadSecurityAPI;
using static Engine.Framework.Opcodes.Client;
using static Engine.Framework.Opcodes.Server;

namespace NetGuard.Engine
{
    public class FixedSizeQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly int _maxSize;

        public FixedSizeQueue(int maxSize)
        {
            _maxSize = maxSize;
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            while (_queue.Count > _maxSize)
            {
                _queue.TryDequeue(out _);
            }
        }

        public IEnumerable<T> GetAllItems()
        {
            return _queue.ToArray();
        }
    }

    class SocketState
    {
        public Socket Socket { get; set; }
        public byte[] Buffer { get; set; }

        public SocketState(Socket socket, byte[] buffer)
        {
            Socket = socket;
            Buffer = buffer;
        }
    }

    sealed class GatewayModule
    {
        private Socket _clientSocket = null;
        private AsyncServer.DelClientDisconnect _delDisconnect;

        object m_Lock = new object();

        Socket _moduleSocket = null;
        AsyncServer.E_ServerType _handlerType;

        byte[] _localBuffer = ArrayPool<byte>.Shared.Rent(8192);
        byte[] _remoteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        Security _localSecurity = new Security();
        Security _remoteSecurity = new Security();

        public static FixedSizeQueue<Packet> _lastPackets = new FixedSizeQueue<Packet>(100);
        private ulong _bytesReceivedFromClient = 0;
        private DateTime _startTime = DateTime.Now;

        private ModuleSettings _moduleSettings = Main._moduleSettings;

        private GatewayClient _client;

        public GatewayModule(Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect)
        {
            _clientSocket = clientSocket;
            _delDisconnect = delDisconnect;
            _handlerType = AsyncServer.E_ServerType.GatewayModule;
            _moduleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _client = new GatewayClient
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
            var remotePackets = _remoteSecurity.TransferIncoming();

            if (remotePackets == null)
                return;

            int count = remotePackets.Count;

            for (int i = 0; i < count; i++)
            {
                var packet = remotePackets[i];

                var msg = $"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}";
                //Custom.WriteLine($"[S->C] {packet.Opcode:X4}", ConsoleColor.DarkMagenta);
                Custom.WriteLine(msg, ConsoleColor.Yellow);

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

                    case SERVER_GATEWAY_LOGIN_RESPONSE:
                        {
                            byte res = packet.ReadUInt8();

                            if (res == 1)
                            {
                                uint id = packet.ReadUInt32();
                                string host = packet.ReadAscii();
                                int port = packet.ReadUInt16();

                                var index = Main._config._agentModules.FindIndex(m => m.moduleIP == host && m.modulePort == port);
                                if (index == -1)
                                {
                                    Custom.WriteLine("Could not find agent bindings", ConsoleColor.Red);
                                    continue;
                                }

                                var guardModule = Main._config._agentModules[index];

                                Custom.WriteLine($"Using {guardModule.guardIP} {guardModule.guardPort}", ConsoleColor.Cyan);

                                Packet spoof = new Packet(0xA102, true);
                                spoof.WriteUInt8(res);
                                spoof.WriteUInt32(id);

                                spoof.WriteAscii(guardModule.guardIP);
                                spoof.WriteUInt16(guardModule.guardPort);
                                spoof.WriteUInt32((uint)0);
                                spoof.Lock();

                                _localSecurity.Send(spoof);
                                Send(false);

                                continue;
                            }




                        }
                        break;

                    default:
                        Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
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

                //Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Yellow);

                var msg = $"[C->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}";
                //Custom.WriteLine($"[S->C] {packet.Opcode:X4}", ConsoleColor.DarkMagenta);
                Custom.WriteLine(msg, ConsoleColor.Yellow);

                //File.AppendAllText($"gateway_{Main.unixTimestamp}_client.txt", "\n" + msg);

                switch (packet.Opcode)
                {
                    case LOGIN_SERVER_HANDSHAKE:
                    case CLIENT_ACCEPT_HANDSHAKE:
                        {
                            Send(false);
                            continue;
                        }

                    case CLIENT_GATEWAY_PATCH_REQUEST:
                        {
                            try
                            {
                                byte contentID = packet.ReadUInt8();
                                string ModuleName = packet.ReadAscii();
                                UInt32 version = packet.ReadUInt32();

                                Custom.WriteLine($"contentID {contentID}");
                                Custom.WriteLine($"ModuleName {ModuleName}");
                                Custom.WriteLine($"version {version}");
                            }
                            catch (Exception ex)
                            {
                                Custom.WriteLine(ex.ToString(), ConsoleColor.Red);
                                Custom.WriteLine($"Wrong packet structure for CLIENT_GATEWAY_PATCH_REQUEST", ConsoleColor.Red);
                                this.DisconnectModuleSocket();
                                return;
                            }
                        }
                        break;

                    case CLIENT_GATEWAY_SHARD_LIST_REQUEST:
                        {
                            if (packet.GetBytes().Length > 0)
                            {
                                Custom.WriteLine($"Ignore packet CLIENT_GATEWAY_SHARD_LIST_REQUEST from {_client.ip}", ConsoleColor.Yellow);
                                continue;
                            }

                            Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);
                        }
                        break;

                    case CLIENT_GATEWAY_LOGIN_REQUEST:
                        {
                            byte locale = packet.ReadUInt8();
                            Custom.WriteLine($"locale: {locale}", ConsoleColor.DarkMagenta);
                            _client.StrUserID = packet.ReadAscii();
                            _client.password = packet.ReadAscii();
                            _client.serverID = packet.ReadUInt16();

                            //if (_client.sent_id != 1 || _client.sent_list != 1)
                            //{
                            //    Custom.WriteLine($"Sent id: {_client.sent_id} Sent list: {_client.sent_list}", ConsoleColor.Red);
                            //    Custom.WriteLine($"Blocked potential exploit from {_client.StrUserID} {_client.password} {_client.ip} for exploiting", ConsoleColor.Yellow);
                            //    continue;
                            //}
                        }
                        break;

                    case CLIENT_GATEWAY_NOTICE_REQUEST:
                        {
                            byte contentID = packet.ReadUInt8();

                            if (packet.GetBytes().Length > 1)
                            {
                                Custom.WriteLine($"Ignore packet CLIENT_GATEWAY_NOTICE_REQUEST from {_client.ip}", ConsoleColor.Yellow);
                                continue;
                            }

                            Custom.WriteLine($"CLIENT_GATEWAY_NOTICE_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);
                        }
                        break;

                    case CLIENT_GATEWAY_SHARD_LIST_PING_REQUEST:
                        {
                            Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_PING_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);
                        }
                        break;

                    case CLIENT_GATEWAY_LOGIN_IBUV_ANSWER:
                        {
                            string code = packet.ReadAscii();
                        }
                        break;

                    case GLOBAL_IDENTIFICATION:
                        {
                            if (packet.GetBytes().Length != 12)
                            {
                                Custom.WriteLine($"Ignore packet GLOBAL_IDENTIFICATION from {_client.ip}", ConsoleColor.Yellow);
                                continue;
                            }

                            DoReceive(false);
                            continue;
                        }

                    case CLIENT_GLOBAL_PING:
                        {
                            if (packet.GetBytes().Length != 0)
                            {
                                Custom.WriteLine($"Ignore packet CLIENT_GLOBAL_PING from {_client.ip}", ConsoleColor.Yellow);
                                continue;
                            }
                        }
                        break;

                    default:
                        {
                            //var test = $"[C->S][{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}";
                            Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                            continue;
                        }
                }

                Packet copyOfPacket = packet;
                _lastPackets.Enqueue(copyOfPacket);
                _remoteSecurity.Send(packet);
                Send(true);
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
            lock (m_Lock)
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
            _delDisconnect?.Invoke(ref _clientSocket, _handlerType);
        }
    }
}