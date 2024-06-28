using SilkroadSecurityAPI;
using NetGuard.Services;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

    sealed class GatewayContext
    {
        private Socket _clientSocket = null;
        AsyncServer.delClientDisconnect _delDisconnect;

        object m_Lock = new object();

        Socket _moduleSocket = null;
        AsyncServer.E_ServerType _handlerType;

        byte[] _localBuffer = ArrayPool<byte>.Shared.Rent(8192);
        byte[] _remoteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        Security _localSecurity = new Security();
        Security _remoteSecurity = new Security();

        //Thread m_TransferPoolThread = null;

        public static FixedSizeQueue<Packet> _lastPackets = new FixedSizeQueue<Packet>(100);
        private ulong _bytesReceivedFromClient = 0;
        private DateTime _startTime = DateTime.Now;

        private GatewayClient _client;

        public GatewayContext(Socket clientSocket, AsyncServer.delClientDisconnect delDisconnect)
        {
            this._clientSocket = clientSocket;
            this._delDisconnect = delDisconnect;
            this._handlerType = AsyncServer.E_ServerType.GatewayServer;
            this._moduleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this._client = new GatewayClient();

            this._client.ip = ((IPEndPoint)_clientSocket.RemoteEndPoint).Address.ToString();

            Custom.WriteLine($"New connection {_client.ip}", ConsoleColor.Cyan);

            try
            {
                this._moduleSocket.Connect(new IPEndPoint(IPAddress.Parse("100.127.205.174"), 5779));
                this._localSecurity.GenerateSecurity(true, true, true);
                this.DoReceiveFromClient();
                Send(false);
            }
            catch
            {
                Custom.WriteLine("Remote host (100.127.205.174:5779) is unreachable.", ConsoleColor.Red);
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
                if(this._moduleSocket != null)
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

        void OnReceiveFromServer(IAsyncResult iar)
        {
            lock (m_Lock)
            {
                try
                {
                    int nReceived = _moduleSocket.EndReceive(iar);
                    if (nReceived != 0)
                    {
                        this._remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
                        var remotePackets = _remoteSecurity.TransferIncoming();

                        if (remotePackets != null)
                        {
                            for (int i = 0; i < remotePackets.Count; i++)
                            {
                                var packet = remotePackets[i];

                                //Custom.WriteLine($"[S->C] {packet.Opcode:X4}", ConsoleColor.DarkMagenta);
                                Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Yellow);

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

                                    default:
                                        Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                                        //Custom.WriteLine($"[S->C] Unknown packet {packet.Opcode:X4} {packet.GetBytes().Length}", ConsoleColor.Yellow);
                                        break;
                                }

                                this._localSecurity.Send(packet);
                                Send(false);
                            }
                        }
                    }
                    else
                    {
                        this.DisconnectModuleSocket();
                        this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
                        return;
                    }

                    DoReceiveFromServer();
                }
                catch
                {
                    this.DisconnectModuleSocket();
                    this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
                }
            }
        }

        void Send(bool toHost)
        {
            lock (m_Lock)
                foreach (var p in (toHost ? _remoteSecurity : _localSecurity).TransferOutgoing())
                {
                    Socket socket = (toHost ? _moduleSocket : _clientSocket);

                    socket.Send(p.Key.Buffer);

                    if (toHost)
                    {
                        try
                        {
                            _bytesReceivedFromClient += (ulong)p.Key.Size;

                            double bytesPerSecond = GetBytesPerSecondFromClient();
                            if (bytesPerSecond > 1000)
                            {
                                Custom.WriteLine($"Client({_client.ip}) disconnected for flooding.", ConsoleColor.Yellow);

                                this.DisconnectModuleSocket();
                                this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
                                return;
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

        void OnReceiveFromClient(IAsyncResult iar)
        {
            lock (m_Lock)
            {
                try
                {
                    int nReceived = _clientSocket.EndReceive(iar);

                    if (nReceived != 0)
                    {
                        _localSecurity.Recv(_localBuffer, 0, nReceived);
                        var receivedPackets = _localSecurity.TransferIncoming();

                        if (receivedPackets != null)
                        {
                            for (int i = 0; i < receivedPackets.Count; i++)
                            {
                                var packet = receivedPackets[i];

                                //Custom.WriteLine($"[C->S] {packet.Opcode:X4}", ConsoleColor.DarkMagenta);
                                Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Yellow);

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

                                            this.DoReceiveFromServer();
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

                                Packet CopyOfPacket = packet;
                                _lastPackets.Enqueue(CopyOfPacket);
                                this._remoteSecurity.Send(packet);
                                Send(true);
                            }
                        }

                    }
                    else
                    {
                        this.DisconnectModuleSocket();
                        this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
                        return;
                    }

                    this.DoReceiveFromClient();
                }
                catch
                {
                    this.DisconnectModuleSocket();
                    this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
                }
            }
        }

        void DoReceiveFromServer()
        {
            try
            {
                this._moduleSocket.BeginReceive(_remoteBuffer, 0, _remoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveFromServer), null);
            }
            catch
            {
                this.DisconnectModuleSocket();
                this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
            }
        }

        void DoReceiveFromClient()
        {
            try
            {
                this._clientSocket.BeginReceive(_localBuffer, 0, _localBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveFromClient), null);
            }
            catch
            {
                this.DisconnectModuleSocket();
                this._delDisconnect.Invoke(ref _clientSocket, _handlerType);
            }
        }
    }
}
