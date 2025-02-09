using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using AgentModule.Classes;
using Module;
using static AgentModule.Framework.Opcodes.Client;
using static AgentModule.Framework.Opcodes.Server;
using System.Linq;
using SilkroadSecurityAPI;
using AgentModule.Engine.Classes;
using AgentModule.Services;
using AgentModule.Framework;
using AgentModule.PacketManager.Client;
using AgentModule.PacketManager.Server;

namespace AgentModule.Engine
{
    sealed class AgentModule
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

        public AgentModule(Socket clientSocket, AsyncServer.DelClientDisconnect delDisconnect)
        {
            _clientSocket = clientSocket;
            _clientDisconnectHandler = delDisconnect;

            _handlerType = AsyncServer.E_ServerType.AgentModule;
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

                if (handler != null)
                {
                    var result = handler.Handle(packet, _client);
                    switch (result.ResultType)
                    {
                        case PacketResultType.Block:
                            Custom.WriteLine($"Prevented [{packet.Opcode:X4}] from being sent from {_client.ip}", ConsoleColor.Red);
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

                            if (errorCode == 0x01)
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
                            Custom.WriteLine($"Prevented [{packet.Opcode:X4}] from being sent from {_client.ip}", ConsoleColor.Red);
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

                        case PacketResultType.DoReceive:
                            DoReceive(false);
                            continue;
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

                if (_client.inCharSelection)
                {
                    switch (packet.Opcode)
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

                            DoReceive(false);
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
                            //Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Red);
                            //continue;
                        }
                        break;
                }

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