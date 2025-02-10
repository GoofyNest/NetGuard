using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPICore;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;

namespace Module.PacketManager.Agent.Server.Handlers
{
    public class CharDataLoadingEnd : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            client.inCharSelection = false;

            var charData = client.charData;

            client.charData.Lock();

            var serverTime = charData.ReadUInt32(); // * 4   uint    ServerTime               //SROTimeStamp
            var refObjId = charData.ReadUInt32(); // 4   uint    RefObjID
            var scale = charData.ReadUInt8(); // 1   byte    Scale
            var curLevel = charData.ReadUInt8(); // 1   byte    CurLevel
            var maxLevel = charData.ReadUInt8(); // 1   byte    MaxLevel
            var expOffset = charData.ReadUInt64(); // 8   ulong   ExpOffset
            var sExpOffset = charData.ReadUInt32(); // 4   uint    SExpOffset
            var remainGold = charData.ReadUInt64(); // 8   ulong   RemainGold
            var remainSkillPoint = charData.ReadUInt32(); // 4   uint    RemainSkillPoint
            var remainStatPoint = charData.ReadUInt16(); // 2   ushort  RemainStatPoint
            var remainHwanCount = charData.ReadUInt8(); // 1   byte    RemainHwanCount
            var gatheredExpPoint = charData.ReadUInt32(); // 4   uint    GatheredExpPoint
            var hp = charData.ReadUInt32(); // 4   uint    HP
            var mp = charData.ReadUInt32(); // 4   uint    MP
            var autoInverstExp = charData.ReadUInt8(); // 1   byte    AutoInverstExp
            var dailyPk = charData.ReadUInt8(); // 1   byte    DailyPK
            var totalPk = charData.ReadUInt16(); // 2   ushort  TotalPK
            var pkPenaltyPoint = charData.ReadUInt32(); // 4   uint    PKPenaltyPoint
            var hwanLevel = charData.ReadUInt8(); // 1   byte    HwanLevel
            var pvpCape = charData.ReadUInt8(); // 1   byte    pvpCape

            // Inventory
            var inventorySize = charData.ReadUInt8(); // 1   byte    Inventory.Size
            var inventoryItemCount = charData.ReadUInt8(); // 1   byte    Inventory.ItemCount
            for (var d = 0; d < inventoryItemCount; d++) // for (int i = 0; i < Inventory.ItemCount; i++)
            {
                var itemSlot = charData.ReadUInt8(); //     1   byte    item.Slot
                var itemRentType = charData.ReadUInt32(); //     4   uint    item.RentType
                if (itemRentType == 1)
                {
                    var itemRentInfoCanDelete = charData.ReadUInt16(); //         2   ushort  item.RentInfo.CanDelete
                    var itemRentInfoPeriodBeginTime =
                        charData.ReadUInt32(); //         4   uint    item.RentInfo.PeriodBeginTime
                    var itemRentInfoPeriodEndTime =
                        charData.ReadUInt32(); //         4   uint    item.RentInfo.PeriodEndTime        
                }
                else if (itemRentType == 2)
                {
                    var itemRentInfoCanDelete = charData.ReadUInt16(); //         2   ushort  item.RentInfo.CanDelete
                    var itemRentInfoCanRecharge = charData.ReadUInt16(); //         2   ushort  item.RentInfo.CanRecharge
                    var itemRentInfoMeterRateTime =
                        charData.ReadUInt32(); //         4   uint    item.RentInfo.MeterRateTime        
                }
                else if (itemRentType == 3)
                {
                    var itemRentInfoCanDelete = charData.ReadUInt16(); //         2   ushort  item.RentInfo.CanDelete
                    var itemRentInfoCanRecharge = charData.ReadUInt16(); //         2   ushort  item.RentInfo.CanRecharge
                    var itemRentInfoPeriodBeginTime =
                        charData.ReadUInt32(); //         4   uint    item.RentInfo.PeriodBeginTime
                    var itemRentInfoPeriodEndTime =
                        charData.ReadUInt32(); //         4   uint    item.RentInfo.PeriodEndTime   
                    var itemRentInfoPackingTime =
                        charData.ReadUInt32(); //         4   uint    item.RentInfo.PackingTime        
                }

                var itemRefItemId = charData.ReadUInt32(); //     4   uint    item.RefItemID

                Console.WriteLine(itemRefItemId);
            }

            return response;
        }
    }
}
