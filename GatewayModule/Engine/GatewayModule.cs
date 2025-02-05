using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
                _moduleSocket.Connect(new IPEndPoint(IPAddress.Parse("100.127.205.174"), 5779));
                _localSecurity.GenerateSecurity(true, true, true);
                DoReceiveFromClient();
                //_ = DoReceiveFromServerAsync();
                Send(false);
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Remote host (100.127.205.174:5779) is unreachable. Exception: {ex}", ConsoleColor.Red);
            }
        }

        private double GetBytesPerSecondFromClient()
        {
            var elapsedTime = DateTime.Now - _startTime;
            return elapsedTime.TotalSeconds < 1.0 ? _bytesReceivedFromClient : Math.Round(_bytesReceivedFromClient / elapsedTime.TotalSeconds, 2);
        }

        private void DisconnectModuleSocket()
        {
            try
            {
                if (this._moduleSocket != null)
                {
                    this._moduleSocket.Close();
                }
                this._moduleSocket = null;
            }
            catch
            {
                Custom.WriteLine("Error occurred while disconnecting module socket.", ConsoleColor.Red);
            }
        }

        void OnReceiveFromServerAsync(IAsyncResult iar)
        {
            lock (m_Lock)
            {
                try
                {
                    int nReceived = _moduleSocket.EndReceive(iar);
                    if (nReceived != 0)
                    {
                        HandleReceivedDataFromServer(nReceived);
                    }
                    else
                    {
                        DisconnectModuleSocket();
                        _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                        return;
                    }

                    DoReceiveFromServer();
                }
                catch (Exception ex)
                {
                    DisconnectModuleSocket();
                    _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                    Custom.WriteLine($"Exception in DoReceiveFromServerAsync: {ex}", ConsoleColor.Red);
                }
            }
        }

        void HandleReceivedDataFromServer(int nReceived)
        {
            _remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
            var remotePackets = _remoteSecurity.TransferIncoming();

            if (remotePackets != null)
            {
                for (int i = 0; i < remotePackets.Count; i++)
                {
                    var packet = remotePackets[i];

                    var msg = $"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}";
                    //Custom.WriteLine($"[S->C] {packet.Opcode:X4}", ConsoleColor.DarkMagenta);
                    Custom.WriteLine(msg, ConsoleColor.Yellow);

                    //File.AppendAllText($"gateway_{Main.unixTimestamp}_server.txt", "\n" + msg);

                    switch (packet.Opcode)
                    {
                        case LOGIN_SERVER_HANDSHAKE:
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

                                if(res == 1)
                                {
                                    uint id = packet.ReadUInt32();
                                    string host = packet.ReadAscii();
                                    int port = packet.ReadUInt16();

                                    var index = Main._config._agentModules.FindIndex(m => m.moduleIP == host && m.modulePort == port);
                                    if(index == -1)
                                        continue;

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
        }

        void Send(bool toHost)
        {
            lock (m_Lock)
            {
                var security = toHost ? _remoteSecurity : _localSecurity;

                if (security == null)
                    return;

                var socket = toHost ? _moduleSocket : _clientSocket;
                if (socket == null)
                    return;

                var outgoing = security.TransferOutgoing();
                if (outgoing == null)
                    return;

                for (var i = 0; i < outgoing.Count; i++)
                {
                    var packet = outgoing[i];

                    try
                    {
                        socket.Send(packet.Key.Buffer);
                        if (toHost)
                        {
                            _bytesReceivedFromClient += (ulong)packet.Key.Size;

                            double bytesPerSecond = GetBytesPerSecondFromClient();
                            if (bytesPerSecond > 1000)
                            {
                                Custom.WriteLine($"Client({_client.ip}) disconnected for flooding.", ConsoleColor.Yellow);

                                this.DisconnectModuleSocket();
                                this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
                                return;
                            }
                        }

                    }
                    catch
                    {
                        this.DisconnectModuleSocket();
                        this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
                    }
                }
            }
        }

        private void OnReceiveFromClientAsync(IAsyncResult iar)
        {
            lock (m_Lock)
            {
                try
                {
                    int nReceived = _clientSocket.EndReceive(iar);
                    if (nReceived != 0)
                    {
                        HandleReceivedDataFromClient(nReceived);
                    }
                    else
                    {
                        DisconnectModuleSocket();
                        _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                        return;
                    }

                    DoReceiveFromClient();
                }
                catch (Exception ex)
                {
                    DisconnectModuleSocket();
                    _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                    Custom.WriteLine($"Exception in DoReceiveFromClientAsync: {ex}", ConsoleColor.Red);
                }
            }
        }

        private void HandleReceivedDataFromClient(int nReceived)
        {
            _localSecurity.Recv(_localBuffer, 0, nReceived);
            var receivedPackets = _localSecurity.TransferIncoming();

            if (receivedPackets != null)
            {
                for (int i = 0; i < receivedPackets.Count; i++)
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

                                if (_client.sent_id != 1 || _client.sent_list != 1)
                                {
                                    Custom.WriteLine($"Disconnected user {_client.StrUserID} {_client.password} {_client.ip} for exploiting", ConsoleColor.Yellow);
                                    this.DisconnectModuleSocket();
                                    return;
                                }
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

                                DoReceiveFromServer();
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
        }

        void DoReceiveFromServer()
        {
            try
            {
                _moduleSocket.BeginReceive(_remoteBuffer, 0, _remoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveFromServerAsync), null);
            }
            catch
            {
                DisconnectModuleSocket();
                _delDisconnect.Invoke(ref _clientSocket, _handlerType);
            }
        }

        void DoReceiveFromClient()
        {
            try
            {
                _clientSocket.BeginReceive(_localBuffer, 0, _localBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveFromClientAsync), null);
            }
            catch
            {
                this.DisconnectModuleSocket();
                this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
            }
        }
    }
}