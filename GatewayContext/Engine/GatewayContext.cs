using Framework;
using NetGuard.Services;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static Engine.Framework.Opcodes;
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

    public sealed class GatewayContext
    {
        private readonly Socket _clientSocket;
        private readonly AsyncServer.DelClientDisconnect _delDisconnect;

        private readonly Socket _moduleSocket;
        private readonly AsyncServer.E_ServerType _handlerType;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly byte[] _localBuffer = ArrayPool<byte>.Shared.Rent(8192);
        private readonly byte[] _remoteBuffer = ArrayPool<byte>.Shared.Rent(8192);

        private readonly Security _localSecurity = new Security();
        private readonly Security _remoteSecurity = new Security();

        public static FixedSizeQueue<Packet> _lastPackets = new FixedSizeQueue<Packet>(100);
        private ulong _bytesReceivedFromClient = 0;
        private readonly DateTime _startTime = DateTime.Now;

        private GatewayClient _client;

        public GatewayContext(Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect)
        {
            _clientSocket = clientSocket;
            _delDisconnect = delDisconnect;
            _handlerType = AsyncServer.E_ServerType.GatewayServer;
            _moduleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _client = new GatewayClient();

            _client.ip = ((IPEndPoint)_clientSocket.RemoteEndPoint).Address.ToString();

            Custom.WriteLine($"New connection {_client.ip}", ConsoleColor.Cyan);

            try
            {
                _moduleSocket.Connect(new IPEndPoint(IPAddress.Parse("100.127.205.174"), 5779));
                _localSecurity.GenerateSecurity(true, true, true);
                DoReceiveFromClientAsync();
                SendAsync(false);
            }
            catch
            {
                Custom.WriteLine("Remote host (127.0.0.1:5779) is unreachable.", ConsoleColor.Red);
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
                _moduleSocket.Close();
            }
            catch
            {
                Custom.WriteLine("Error occurred while disconnecting module socket.", ConsoleColor.Red);
            }
        }

        private async void OnReceiveFromServerAsync(IAsyncResult iar)
        {
            await _semaphore.WaitAsync();

            try
            {
                int nReceived = _moduleSocket.EndReceive(iar);
                if (nReceived <= 0)
                {
                    DisconnectModuleSocket();
                    _delDisconnect.Invoke(_clientSocket, _handlerType);
                    return;
                }

                _remoteSecurity.Recv(_remoteBuffer, 0, nReceived);
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
                            case Server.LOGIN_SERVER_HANDSHAKE:
                                {
                                    SendAsync(true);
                                    continue;
                                }

                            case Server.SERVER_GATEWAY_PATCH_RESPONSE:
                                _client.sent_id = 1;
                                break;

                            case Server.SERVER_GATEWAY_SHARD_LIST_RESPONSE:
                                _client.sent_list = 1;
                                break;

                            default:
                                Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                                //Custom.WriteLine($"[S->C] Unknown packet {packet.Opcode:X4} {packet.GetBytes().Length}", ConsoleColor.Yellow);
                                break;
                        }

                        _localSecurity.Send(packet);
                        SendAsync(false);
                    }
                }

                DoReceiveFromServerAsync();
            }
            catch
            {
                DisconnectModuleSocket();
                _delDisconnect.Invoke(_clientSocket, _handlerType);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async void SendAsync(bool toHost)
        {
            await _semaphore.WaitAsync();

            try
            {
                var security = toHost ? _remoteSecurity : _localSecurity;
                var socket = toHost ? _moduleSocket : _clientSocket;

                var outgoingPackets = security.TransferOutgoing();
                for (int i = 0; i < outgoingPackets.Count; i++)
                {
                    var packet = outgoingPackets[i];
                    await socket.SendAsync(new ArraySegment<byte>(packet.Key.Buffer), SocketFlags.None);
                    if (toHost)
                    {
                        _bytesReceivedFromClient += (ulong)packet.Key.Size;

                        var bytesPerSecond = GetBytesPerSecondFromClient();
                        if (bytesPerSecond > 1000)
                        {
                            Custom.WriteLine($"Client({_client.ip}) disconnected for flooding.", ConsoleColor.Yellow);
                            DisconnectModuleSocket();
                            _delDisconnect.Invoke(_clientSocket, _handlerType);
                            return;
                        }
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async void OnReceiveFromClientAsync(IAsyncResult iar)
        {
            await _semaphore.WaitAsync();

            try
            {
                int nReceived = _clientSocket.EndReceive(iar);

                if (nReceived <= 0)
                {
                    DisconnectModuleSocket();
                    _delDisconnect.Invoke(_clientSocket, _handlerType);
                    return;
                }

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
                            case Client.LOGIN_SERVER_HANDSHAKE:
                            case Client.CLIENT_ACCEPT_HANDSHAKE:
                                {
                                    SendAsync(false);
                                    continue;
                                }

                            case Client.CLIENT_GATEWAY_PATCH_REQUEST:
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
                                        DisconnectModuleSocket();
                                        return;
                                    }
                                }
                                break;

                            case Client.CLIENT_GATEWAY_SHARD_LIST_REQUEST:
                                {
                                    if(packet.GetBytes().Length > 0)
                                    {
                                        Custom.WriteLine($"Ignore packet CLIENT_GATEWAY_SHARD_LIST_REQUEST from {_client.ip}", ConsoleColor.Yellow);
                                        continue;
                                    }

                                    Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);
                                }
                                break;

                            case Client.CLIENT_GATEWAY_LOGIN_REQUEST:
                                {
                                    byte locale = packet.ReadUInt8();
                                    _client.StrUserID = packet.ReadAscii();
                                    _client.password = packet.ReadAscii();
                                    _client.serverID = packet.ReadUInt16();

                                    if (_client.sent_id != 1 || _client.sent_list != 1)
                                    {
                                        Custom.WriteLine($"Disconnected user {_client.StrUserID} {_client.password} {_client.ip} for exploiting", ConsoleColor.Yellow);
                                        DisconnectModuleSocket();
                                        return;
                                    }
                                }
                                break;

                            case Client.CLIENT_GATEWAY_NOTICE_REQUEST:
                                {
                                    byte contentID = packet.ReadUInt8();

                                    if(packet.GetBytes().Length > 1)
                                    {
                                        Custom.WriteLine($"Ignore packet CLIENT_GATEWAY_NOTICE_REQUEST from {_client.ip}", ConsoleColor.Yellow);
                                        continue;
                                    }

                                    Custom.WriteLine($"CLIENT_GATEWAY_NOTICE_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);
                                }
                                break;

                            case Client.CLIENT_GATEWAY_SHARD_LIST_PING_REQUEST:
                                {
                                    Custom.WriteLine($"CLIENT_GATEWAY_SHARD_LIST_PING_REQUEST {packet.GetBytes().Length}", ConsoleColor.DarkMagenta);
                                }
                                break;

                            case Client.CLIENT_GATEWAY_LOGIN_IBUV_ANSWER:
                                {
                                    string code = packet.ReadAscii();
                                }
                                break;

                            case Client.GLOBAL_IDENTIFICATION:
                                {
                                    if (packet.GetBytes().Length != 12)
                                    {
                                        Custom.WriteLine($"Ignore packet GLOBAL_IDENTIFICATION from {_client.ip}", ConsoleColor.Yellow);
                                        continue;
                                    }

                                    DoReceiveFromServerAsync();
                                    continue;
                                }

                            case Client.CLIENT_GLOBAL_PING:
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
                        _remoteSecurity.Send(packet);
                        SendAsync(true);
                    }
                }

                DoReceiveFromClientAsync();
            }
            catch
            {
                DisconnectModuleSocket();
                _delDisconnect.Invoke(_clientSocket, _handlerType);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async void DoReceiveFromServerAsync()
        {
            await _semaphore.WaitAsync();

            try
            {
                _moduleSocket.BeginReceive(_remoteBuffer, 0, _remoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveFromServerAsync), null);
            }
            catch
            {
                DisconnectModuleSocket();
                _delDisconnect.Invoke(_clientSocket, _handlerType);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async void DoReceiveFromClientAsync()
        {
            await _semaphore.WaitAsync();

            try
            {
                _clientSocket.BeginReceive(_localBuffer, 0, _localBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveFromClientAsync), null);
            }
            catch
            {
                DisconnectModuleSocket();
                _delDisconnect.Invoke(_clientSocket, _handlerType);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
