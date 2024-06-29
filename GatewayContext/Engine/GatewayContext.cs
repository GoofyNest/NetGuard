using NetGuard.Services;
using SilkroadSecurityAPI;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
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
        private Socket _clientSocket;
        private readonly AsyncServer.DelClientDisconnect _delDisconnect;
        private readonly object _lock = new object();
        private Socket _moduleSocket;
        private readonly AsyncServer.E_ServerType _handlerType;

        private byte[] _localBuffer = ArrayPool<byte>.Shared.Rent(8192);
        private byte[] _remoteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        private readonly Security _localSecurity = new Security();
        private readonly Security _remoteSecurity = new Security();

        public static FixedSizeQueue<Packet> _lastPackets = new FixedSizeQueue<Packet>(100);
        private ulong _bytesReceivedFromClient = 0;
        private readonly DateTime _startTime = DateTime.Now;
        private GatewayClient _client;

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public GatewayContext(Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect)
        {
            _clientSocket = clientSocket;
            _delDisconnect = delDisconnect;
            _handlerType = AsyncServer.E_ServerType.GatewayServer;
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
                _ = DoReceiveFromClientAsync();
                //_ = DoReceiveFromServerAsync();
                _ = SendAsync(false);
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
                _moduleSocket?.Close();
                _moduleSocket = null;
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Error occurred while disconnecting module socket. Exception: {ex}", ConsoleColor.Red);
            }
        }

        private async Task DoReceiveFromServerAsync()
        {
            try
            {
                while (true)
                {
                    int nReceived = await _moduleSocket.ReceiveAsync(new ArraySegment<byte>(_remoteBuffer), SocketFlags.None);
                    if (nReceived > 0)
                    {
                        HandleReceivedDataFromServer(nReceived);
                    }
                    else
                    {
                        DisconnectModuleSocket();
                        _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                DisconnectModuleSocket();
                _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                Custom.WriteLine($"Exception in DoReceiveFromServerAsync: {ex}", ConsoleColor.Red);
            }
        }

        private void HandleReceivedDataFromServer(int nReceived)
        {
            lock (_lock)
            {
                _remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
                var remotePackets = _remoteSecurity.TransferIncoming();

                if (remotePackets != null)
                {
                    for (int i = 0; i < remotePackets.Count; i++)
                    {
                        var packet = remotePackets[i];
                        Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Yellow);

                        switch (packet.Opcode)
                        {
                            case LOGIN_SERVER_HANDSHAKE:
                                {
                                    _ = SendAsync(true);
                                    continue;
                                }
                        }

                        _localSecurity.Send(packet);
                        _ = SendAsync(false);
                    }
                }
            }
        }

        private async Task SendAsync(bool toHost)
        {
            await _sendLock.WaitAsync();
            try
            {
                var security = toHost ? _remoteSecurity : _localSecurity;
                foreach (var packet in security.TransferOutgoing())
                {
                    var socket = toHost ? _moduleSocket : _clientSocket;

                    await socket.SendAsync(new ArraySegment<byte>(packet.Key.Buffer), SocketFlags.None);

                    if (toHost)
                    {
                        try
                        {
                            _bytesReceivedFromClient += (ulong)packet.Key.Size;
                            double bytesPerSecond = GetBytesPerSecondFromClient();
                            if (bytesPerSecond > 1000)
                            {
                                Custom.WriteLine($"Client({_client.ip}) disconnected for flooding.", ConsoleColor.Yellow);

                                DisconnectModuleSocket();
                                _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Custom.WriteLine($"Exception in SendAsync (toHost): {ex}", ConsoleColor.Red);
                            DisconnectModuleSocket();
                            _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                        }
                    }
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private async Task DoReceiveFromClientAsync()
        {
            try
            {
                while (true)
                {
                    int nReceived = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(_localBuffer), SocketFlags.None);
                    if (nReceived > 0)
                    {
                        HandleReceivedDataFromClient(nReceived);
                    }
                    else
                    {
                        DisconnectModuleSocket();
                        _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                DisconnectModuleSocket();
                _delDisconnect.Invoke(ref _clientSocket, _handlerType);
                Custom.WriteLine($"Exception in DoReceiveFromClientAsync: {ex}", ConsoleColor.Red);
            }
        }

        private async Task HandleReceivedDataFromClient(int nReceived)
        {
            lock (_lock)
            {
                _localSecurity.Recv(_localBuffer, 0, nReceived);
                var receivedPackets = _localSecurity.TransferIncoming();

                if (receivedPackets != null)
                {
                    for (int i = 0; i < receivedPackets.Count; i++)
                    {
                        var packet = receivedPackets[i];
                        Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Yellow);

                        switch (packet.Opcode)
                        {
                            case LOGIN_SERVER_HANDSHAKE:
                            case CLIENT_ACCEPT_HANDSHAKE:
                                {
                                    _ = SendAsync(false);
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

                                    _ = DoReceiveFromServerAsync();
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
                        _ = SendAsync(true);
                    }
                }
            }
        }
    }
}