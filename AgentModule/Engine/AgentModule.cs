﻿using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NetGuard.Services;
using SilkroadSecurityAPI;
using static Engine.Framework.Opcodes.Client;
using static Engine.Framework.Opcodes.Server;

namespace NetGuard.Engine
{
    public class Globals
    {
        public string aids { get; set; }
    }

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

    sealed class AgentModule
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

        private AgentClient _client;

        public AgentModule(Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect)
        {
            _clientSocket = clientSocket;
            _delDisconnect = delDisconnect;
            _handlerType = AsyncServer.E_ServerType.AgentModule;
            _moduleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _client = new AgentClient
            {
                ip = ((IPEndPoint)_clientSocket.RemoteEndPoint).Address.ToString()
            };

            Custom.WriteLine($"New connection {_client.ip}", ConsoleColor.Cyan);

            try
            {
                _moduleSocket.Connect(new IPEndPoint(IPAddress.Parse("100.127.205.174"), 5884));
                _localSecurity.GenerateSecurity(true, true, true);
                DoReceiveFromClient();
                //_ = DoReceiveFromServerAsync();
                Send(false);
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Remote host (100.127.205.174:5884) is unreachable. Exception: {ex}", ConsoleColor.Red);
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

                    //File.AppendAllText($"agent_{Main.unixTimestamp}_server.txt", "\n" + msg);

                    switch (packet.Opcode)
                    {
                        case 0x34A5: 
                            {
                                _client.charData = null;
                            }
                            break;

                        case SERVER_LOADING_END:
                            {
                                _client.inCharSelection = false;

                                var _packet = _client.charData;

                                _client.charData.Lock();

                                var serverTime = _packet.ReadUInt32(); // * 4   uint    ServerTime               //SROTimeStamp
                                var refObjId = _packet.ReadUInt32(); // 4   uint    RefObjID
                                var scale = _packet.ReadUInt8(); // 1   byte    Scale
                                var curLevel = _packet.ReadUInt8(); // 1   byte    CurLevel
                                var maxLevel = _packet.ReadUInt8(); // 1   byte    MaxLevel
                                var expOffset = _packet.ReadUInt64(); // 8   ulong   ExpOffset
                                var sExpOffset = _packet.ReadUInt32(); // 4   uint    SExpOffset
                                var remainGold = _packet.ReadUInt64(); // 8   ulong   RemainGold
                                var remainSkillPoint = _packet.ReadUInt32(); // 4   uint    RemainSkillPoint
                                var remainStatPoint = _packet.ReadUInt16(); // 2   ushort  RemainStatPoint
                                var remainHwanCount = _packet.ReadUInt8(); // 1   byte    RemainHwanCount
                                var gatheredExpPoint = _packet.ReadUInt32(); // 4   uint    GatheredExpPoint
                                var hp = _packet.ReadUInt32(); // 4   uint    HP
                                var mp = _packet.ReadUInt32(); // 4   uint    MP
                                var autoInverstExp = _packet.ReadUInt8(); // 1   byte    AutoInverstExp
                                var dailyPk = _packet.ReadUInt8(); // 1   byte    DailyPK
                                var totalPk = _packet.ReadUInt16(); // 2   ushort  TotalPK
                                var pkPenaltyPoint = _packet.ReadUInt32(); // 4   uint    PKPenaltyPoint
                                var hwanLevel = _packet.ReadUInt8(); // 1   byte    HwanLevel
                                var pvpCape = _packet.ReadUInt8(); // 1   byte    pvpCape

                                // Inventory
                                var inventorySize = _packet.ReadUInt8(); // 1   byte    Inventory.Size
                                var inventoryItemCount = _packet.ReadUInt8(); // 1   byte    Inventory.ItemCount
                                for (var d = 0; d < inventoryItemCount; d++) // for (int i = 0; i < Inventory.ItemCount; i++)
                                {
                                    var itemSlot = _packet.ReadUInt8(); //     1   byte    item.Slot
                                    var itemRentType = _packet.ReadUInt32(); //     4   uint    item.RentType
                                    if (itemRentType == 1)
                                    {
                                        var itemRentInfoCanDelete = _packet.ReadUInt16(); //         2   ushort  item.RentInfo.CanDelete
                                        var itemRentInfoPeriodBeginTime =
                                            _packet.ReadUInt32(); //         4   uint    item.RentInfo.PeriodBeginTime
                                        var itemRentInfoPeriodEndTime =
                                            _packet.ReadUInt32(); //         4   uint    item.RentInfo.PeriodEndTime        
                                    }
                                    else if (itemRentType == 2)
                                    {
                                        var itemRentInfoCanDelete = _packet.ReadUInt16(); //         2   ushort  item.RentInfo.CanDelete
                                        var itemRentInfoCanRecharge = _packet.ReadUInt16(); //         2   ushort  item.RentInfo.CanRecharge
                                        var itemRentInfoMeterRateTime =
                                            _packet.ReadUInt32(); //         4   uint    item.RentInfo.MeterRateTime        
                                    }
                                    else if (itemRentType == 3)
                                    {
                                        var itemRentInfoCanDelete = _packet.ReadUInt16(); //         2   ushort  item.RentInfo.CanDelete
                                        var itemRentInfoCanRecharge = _packet.ReadUInt16(); //         2   ushort  item.RentInfo.CanRecharge
                                        var itemRentInfoPeriodBeginTime =
                                            _packet.ReadUInt32(); //         4   uint    item.RentInfo.PeriodBeginTime
                                        var itemRentInfoPeriodEndTime =
                                            _packet.ReadUInt32(); //         4   uint    item.RentInfo.PeriodEndTime   
                                        var itemRentInfoPackingTime =
                                            _packet.ReadUInt32(); //         4   uint    item.RentInfo.PackingTime        
                                    }

                                    var itemRefItemId = _packet.ReadUInt32(); //     4   uint    item.RefItemID

                                    Console.WriteLine(itemRefItemId);
                                }
                            }
                            break;

                        case 0x3013:
                            {
                                if (_client.charData == null)
                                {
                                    _client.charData = new Packet(0x0000);
                                }

                                for (var d = 0; d < packet.GetBytes().Length; d++)
                                    _client.charData.WriteUInt8(packet.ReadUInt8());
                            }
                            break;


                        case LOGIN_SERVER_HANDSHAKE:
                            {
                                Send(true);
                                continue;
                            }

                        case CLIENT_GLOBAL_PING:
                            {

                            }
                            break;

                        case SERVER_AGENT_ENVIROMMENT_CELESTIAL_POSITION:
                            {
                                UInt32 uniqueID = packet.ReadUInt32();
                                ushort moonPhase = packet.ReadUInt16();
                                byte hour = packet.ReadUInt8();
                                byte minute = packet.ReadUInt8();

                                _client.exploitIwaFix = false;
                            }
                            break;

                        case SERVER_AGENT_CHARACTER_SELECTION_RESPONSE:
                            _client.inCharSelection = true;
                            break;

                        case SERVER_AGENT_AUTH_RESPONSE:
                            {
                                byte errorCode = packet.ReadUInt8();

                                if(errorCode == 0x01)
                                    _client.inCharSelection = true;

                                Custom.WriteLine($"SERVER_AGENT_AUTH_RESPONSE {errorCode}", ConsoleColor.Cyan);
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

                    var msg = $"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}";
                    //Custom.WriteLine($"[S->C] {packet.Opcode:X4}", ConsoleColor.DarkMagenta);
                    Custom.WriteLine(msg, ConsoleColor.Yellow);

                    //File.AppendAllText($"agent_{Main.unixTimestamp}_client.txt", "\n" + msg);

                    Packet copyOfPacket = packet;

                    if (_client.inCharSelection)
                    {
                        switch(packet.Opcode)
                        {
                            case CLIENT_GLOBAL_PING:
                            case CLIENT_AGENT_CHARACTER_SELECTION_JOIN_REQUEST:
                            case CLIENT_AGENT_CHARACTER_SELECTION_ACTION_REQUEST:
                                {
                                    _lastPackets.Enqueue(copyOfPacket);
                                    _remoteSecurity.Send(packet);
                                    Send(true);
                                }
                                break;
                        
                            default:
                                {
                                    Custom.WriteLine($"Ignore packet {packet.Opcode:X4} from {_client.ip} Reason: Char Selection", ConsoleColor.DarkRed);
                                }
                                break;
                        }
                        continue;
                    }

                    switch (packet.Opcode)
                    {
                        case 0x3012:
                            {

                            }
                            break;

                        case 0x3014:
                            {
                                Custom.WriteLine("Different locale shit?");
                                Packet spoof = new Packet(0x3012);
                                _remoteSecurity.Send(spoof);
                                Send(false);
                                continue;
                            }

                        case LOGIN_SERVER_HANDSHAKE:
                        case CLIENT_ACCEPT_HANDSHAKE:
                            {
                                Send(false);
                                continue;
                            }

                        case CLIENT_AGENT_LOGOUT_REQUEST:
                            {
                                if (_client.exploitIwaFix)
                                {
                                    Custom.WriteLine($"Ignore packet 0x{packet.Opcode:X4} from {_client.ip} Reason: Iwa exploit fix", ConsoleColor.DarkRed);
                                    continue;
                                }

                                byte action = packet.ReadUInt8();
                                switch (action)
                                {
                                    case 0x01:
                                        // Exit delay
                                        break;
                                    case 0x02:
                                        // Restart delay
                                        break;

                                    default:
                                        {
                                            Custom.WriteLine($"Ignore packet 0x{packet.Opcode:X4} from {_client.ip} Reason: Iwa exploit fix", ConsoleColor.DarkRed);
                                            continue;
                                        }
                                }
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

                        case CLIENT_AGENT_AUTH_REQUEST:
                            {
                                UInt32 Token = packet.ReadUInt32(); //from LOGIN_RESPONSE
                                _client.StrUserID = packet.ReadAscii();
                                _client.password = packet.ReadAscii();
                                byte OperationType = packet.ReadUInt8();

                                byte[] mac = packet.ReadUInt8Array(6);
                                string mac_address = BitConverter.ToString(mac);
                                int fail_count = mac.Count(b => b == 0x00);

                                _client.mac = mac_address;
                            }
                            break;

                        default:
                            {
                                //var test = $"[C->S][{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}";
                                Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                                //continue;
                            }
                            break;
                    }

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