using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;
namespace Module.Helpers.PacketManager.Agent.Server.Handlers
{
    public class CharDataLoadingEnd : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            /*
                The CharData packet I am using is from DuckSoup's project
                https://github.com/ducksoup-sro/ducksoup

                I fixed the CharData parsing for JSRO files since most of the stuff is missing in older files.

                <3
            */
            PacketHandlingResult response = new PacketHandlingResult();

            client.Agent.InCharSelectionScreen = false;
            client.Agent.IsIngame = true;

            client.Agent.SentJoinRequest = false;

            var charData = client.Agent.CharData;

            charData.Lock();

            var _settings = Main._settings;
            var _files = _settings.ServerType.CurrentValue;

            bool vSRO = _files == "vSRO";

            var _playerInfo = client.PlayerInfo;

            var charIndex = _playerInfo.CharInfo.FindIndex(m => m.Charname == _playerInfo.CurrentCharName);

            var _char = _playerInfo.CharInfo[charIndex];

            _char.ServerTime = charData.ReadUInt32();
            _char.RefObjID = charData.ReadUInt32();
            _char.Scale = charData.ReadUInt8();
            _char.CurLevel = charData.ReadUInt8();
            _char.MaxLevel = charData.ReadUInt8();
            _char.ExpOffset = charData.ReadUInt64();
            _char.SExpOffset = charData.ReadUInt32();
            _char.RemainGold = charData.ReadUInt64();
            _char.RemainSkillPoint = charData.ReadUInt32();
            _char.RemainStatPoint = charData.ReadUInt16();
            _char.RemainHwanCount = charData.ReadUInt8();

            if(vSRO)
            {
                _char.GatheredExpPoint = charData.ReadUInt32();
            }

            _char.Hp = charData.ReadUInt32();
            _char.Mp = charData.ReadUInt32();
            _char.AutoInverstExp = charData.ReadUInt8();
            _char.DailyPk = charData.ReadUInt8();
            _char.TotalPk = charData.ReadUInt16();
            _char.PkPenaltyPoint = charData.ReadUInt32();

            if(vSRO)
            {
                _char.HwanLevel = charData.ReadUInt8();
                _char.PvpCape = charData.ReadUInt8();
            }

            _char.Inventory = new();

            var _inv = _char.Inventory;

            _inv.Size = charData.ReadUInt8();
            _inv.ItemCount = charData.ReadUInt8();

            var _allItems = Main._items;

            if(_allItems.Count == 0)
            {
                Custom.WriteLine($"Please extract itemdata from Client and put it inside {_settings.Data.Path}");
                return response;
            }

            if (_inv.ItemCount > 0)
                _inv.Items = new();

            for (var d = 0; d < _inv.ItemCount; d++)
            {
                var _tempItem = new Item();

                _tempItem.Slot = charData.ReadUInt8();

                if(vSRO)
                {
                    var itemRentType = charData.ReadUInt32();

                    if (itemRentType == 1)
                    {
                        var itemRentInfoCanDelete = charData.ReadUInt16(); //2   ushort  item.RentInfo.CanDelete
                        var itemRentInfoPeriodBeginTime =
                            charData.ReadUInt32(); //4   uint    item.RentInfo.PeriodBeginTime
                        var itemRentInfoPeriodEndTime =
                            charData.ReadUInt32(); //4   uint    item.RentInfo.PeriodEndTime        
                    }
                    else if (itemRentType == 2)
                    {
                        var itemRentInfoCanDelete = charData.ReadUInt16(); //2   ushort  item.RentInfo.CanDelete
                        var itemRentInfoCanRecharge = charData.ReadUInt16(); //2   ushort  item.RentInfo.CanRecharge
                        var itemRentInfoMeterRateTime =
                            charData.ReadUInt32(); //4   uint    item.RentInfo.MeterRateTime        
                    }
                    else if (itemRentType == 3)
                    {
                        var itemRentInfoCanDelete = charData.ReadUInt16(); //2   ushort  item.RentInfo.CanDelete
                        var itemRentInfoCanRecharge = charData.ReadUInt16(); //2   ushort  item.RentInfo.CanRecharge
                        var itemRentInfoPeriodBeginTime =
                            charData.ReadUInt32(); //4   uint    item.RentInfo.PeriodBeginTime
                        var itemRentInfoPeriodEndTime =
                            charData.ReadUInt32(); //4   uint    item.RentInfo.PeriodEndTime   
                        var itemRentInfoPackingTime =
                            charData.ReadUInt32(); //4   uint    item.RentInfo.PackingTime        
                    }
                }

                _tempItem.Id = charData.ReadUInt32(); //4   uint    item.RefItemID

                var itemFound = _allItems.TryGetValue((int)_tempItem.Id, out var _foundItem);

                if (!itemFound)
                    continue;

                if (_foundItem == null)
                    continue;

                _tempItem.CodeName128 = _foundItem.CodeName128;

                if (_foundItem.TypeID1 == 3)
                {
                    //ITEM_
                    if (_foundItem.TypeID2 == 1)
                    {
                        //ITEM_CH
                        //ITEM_EU
                        //AVATAR_
                        _tempItem.OptLevel = charData.ReadUInt8();
                        _tempItem.Variance = charData.ReadUInt64();
                        _tempItem.Durability = charData.ReadUInt32();
                        _tempItem.MagParamNum = charData.ReadUInt8();

                        for (var paramIndex = 0; paramIndex < _tempItem.MagParamNum; paramIndex++)
                        {
                            var magParamType = charData.ReadUInt32(); // 4   uint    magParam.Type
                            var magParamValue = charData.ReadUInt32(); // 4   uint    magParam.Value                
                        }

                        if(vSRO)
                        {
                            var bindingOptionType = charData.ReadUInt8(); // 1   byte    bindingOptionType   //1 = Socket
                            var bindingOptionCount = charData.ReadUInt8(); // 1   byte    bindingOptionCount
                            for (var bindingOptionIndex = 0; bindingOptionIndex < bindingOptionCount; bindingOptionIndex++)
                            {
                                var bindingOptionSlot = charData.ReadUInt8(); // 1   byte bindingOption.Slot
                                var bindingOptionId = charData.ReadUInt32(); // 4   uint bindingOption.ID
                                var bindingOptionParam1 = charData.ReadUInt32(); // 4   uint bindingOption.nParam1
                            }

                            var bindingOptionType2 =
                                charData.ReadUInt8(); // 1   byte    bindingOptionType   //2 = Advanced elixir
                            var bindingOptionCount2 = charData.ReadUInt8(); // 1   byte    bindingOptionCount2
                            for (var bindingOptionIndex = 0; bindingOptionIndex < bindingOptionCount2; bindingOptionIndex++)
                            {
                                var bindingOptionSlot = charData.ReadUInt8(); // 1   byte bindingOption.Slot
                                var bindingOptionId = charData.ReadUInt32(); // 4   uint bindingOption.ID
                                var bindingOptionOptValue = charData.ReadUInt32(); // 4   uint bindingOption.OptValue
                            }
                        }

                        //Custom.WriteLine($"ITEM_ {_tempItem.Slot} {_tempItem.Id} {_tempItem.CodeName128}");
                    }
                    else if (_foundItem.TypeID2 == 2)
                    {
                        if (_foundItem.TypeID3 == 1) //ITEM_COS_P
                        {
                            Custom.WriteLine("//ITEM_COS_P called");
                            var cosState = charData.ReadUInt8(); //1   byte    State
                            if (cosState == 2 || cosState == 3 || cosState == 4)
                            {
                                Custom.WriteLine("cosState == 2 || cosState == 3 || cosState == 4");
                                var cosRefObjId = charData.ReadUInt32(); // 4 uint RefObjID
                                var cosName = charData.ReadAscii(); // 2 ushort Name.Length //     * string Name

                                if (_foundItem.TypeID4 == 2)
                                {
                                    //ITEM_COS_P (Ability)
                                    var cosSecondsToRentEndTime = charData.ReadUInt32(); // 4 uint SecondsToRentEndTime
                                }

                                // Maybe?!
                                // might be service thing
                                var hasInventoryTime = charData.ReadUInt8(); // 1 byte unkByte0

                                if (hasInventoryTime == 0x1)
                                {
                                    // Perhaps inventory span
                                    var unk1222 = charData.ReadUInt8(); // NANI
                                    var unk1223 = charData.ReadUInt32(); // THE
                                    var unk1224 = charData.ReadUInt32(); // FUCK
                                    if (unk1224 == 5)
                                    {
                                        // Special Thanks to BimBum1337
                                        // https://i.rapture.pw/BAKE5/ZEqISAFo33.png/raw
                                        var unk1225 = charData.ReadUInt32(); // ?!
                                        var unk1226 = charData.ReadUInt8();
                                    }
                                }
                            }
                        }
                        else if (_foundItem.TypeID3 == 2)
                        {
                            //ITEM_ETC_TRANS_MONSTER
                            var etcRefObjId = charData.ReadUInt32(); // 4   uint    RefObjID
                            Custom.WriteLine("ITEM_ETC_TRANS_MONSTER");
                        }
                        else if (_foundItem.TypeID3 == 3)
                        {
                            Custom.WriteLine("MAGIC_CUBE");
                            //MAGIC_CUBE
                            var quantity =
                                charData
                                    .ReadUInt32(); // 4   uint    Quantity        //Do not confuse with StackCount, this indicates the amount of elixirs in the cube
                        }
                    }
                    else if (_foundItem.TypeID2 == 3)
                    {
                        //ITEM_ETC
                        var itemStackCount = charData.ReadUInt16(); // 2   ushort  item.StackCount

                        //Custom.WriteLine($"ITEM_ETC: {_tempItem.Slot} {_tempItem.Id} {_tempItem.CodeName128}");

                        if (_foundItem.TypeID3 == 11)
                        {
                            if (_foundItem.TypeID4 == 1 || _foundItem.TypeID4 == 2)
                            {
                                //MAGICSTONE, ATTRSTONE
                                var attributeAssimilationProbability =
                                    charData.ReadUInt8(); // 1   byte    AttributeAssimilationProbability
                            }
                        }
                        else if (_foundItem.TypeID3 == 14 && _foundItem.TypeID4 == 2)
                        {
                            //ITEM_MALL_GACHA_CARD_WIN
                            //ITEM_MALL_GACHA_CARD_LOSE
                            var magParamNum = charData.ReadUInt8(); // 1   byte    item.MagParamCount
                            for (var paramIndex = 0; paramIndex < magParamNum; paramIndex++)
                            {
                                var magParamType = charData.ReadUInt32(); //4   uint magParam.Type
                                var magParamValue = charData.ReadUInt32(); //4   uint magParam.Value
                            }
                        }
                    }
                }

                _inv.Items.Add(_tempItem);
            }

            //var packetList = new List<PacketList>();

            //foreach(var dd in _inv.Items)
            //{
            //    Custom.WriteLine($"{dd.ToString()}");
            //}

            return response;
        }
    }
}
