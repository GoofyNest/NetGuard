using NetGuard.Services;
using SilkroadSecurityAPI;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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

    sealed class AgentContext
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

        private AgentClient _client;

        public AgentContext(Socket clientSocket, AsyncServer.delClientDisconnect delDisconnect)
        {
            this._clientSocket = clientSocket;
            this._delDisconnect = delDisconnect;
            this._handlerType = AsyncServer.E_ServerType.AgentServer;
            this._moduleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this._client = new AgentClient();

            this._client.ip = ((IPEndPoint)_clientSocket.RemoteEndPoint).Address.ToString();

            Custom.WriteLine($"New connection {_client.ip}", ConsoleColor.Cyan);

            try
            {
                this._moduleSocket.Connect(new IPEndPoint(IPAddress.Parse("100.127.205.174"), 5884));
                this._localSecurity.GenerateSecurity(true, true, true);
                this.DoReceiveFromClient();
                Send(false);
            }
            catch
            {
                Custom.WriteLine("Remote host (100.127.205.174:5884) is unreachable.", ConsoleColor.Red);
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