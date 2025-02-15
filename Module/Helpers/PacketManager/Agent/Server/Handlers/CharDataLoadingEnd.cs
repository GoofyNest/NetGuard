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

            response.ModifiedPacket = null!;

            client.agentSettings.inCharSelectionScreen = false;
            client.agentSettings.isIngame = true;


            client.agentSettings.sentJoinRequest = false;

            var charData = client.agentSettings.charData;

            charData.Lock();

            var _settings = Main._settings;
            var _files = _settings.serverVersion.CurrentValue;

            bool vSRO = _files == "vSRO";

            var _playerInfo = client.playerInfo;

            var charIndex = _playerInfo.charInfo.FindIndex(m => m.charname == _playerInfo.currentChar);

            var _char = _playerInfo.charInfo[charIndex];

            _char.serverTime = charData.ReadUInt32();
            _char.refObjID = charData.ReadUInt32();
            _char.scale = charData.ReadUInt8();
            _char.curLevel = charData.ReadUInt8();
            _char.maxLevel = charData.ReadUInt8();
            _char.expOffset = charData.ReadUInt64();
            _char.sExpOffset = charData.ReadUInt32();
            _char.remainGold = charData.ReadUInt64();
            _char.remainSkillPoint = charData.ReadUInt32();
            _char.remainStatPoint = charData.ReadUInt16();
            _char.remainHwanCount = charData.ReadUInt8();

            if(vSRO)
            {
                _char.gatheredExpPoint = charData.ReadUInt32();
            }

            _char.hp = charData.ReadUInt32();
            _char.mp = charData.ReadUInt32();
            _char.autoInverstExp = charData.ReadUInt8();
            _char.dailyPk = charData.ReadUInt8();
            _char.totalPk = charData.ReadUInt16();
            _char.pkPenaltyPoint = charData.ReadUInt32();

            if(vSRO)
            {
                _char.hwanLevel = charData.ReadUInt8();
                _char.pvpCape = charData.ReadUInt8();
            }

            _char.inventory = new();

            var _inv = _char.inventory;

            _inv.size = charData.ReadUInt8();
            _inv.itemCount = charData.ReadUInt8();

            var _allItems = Main._items;

            if(_allItems.Count == 0)
            {
                Custom.WriteLine($"Please extract itemdata from Client and put it inside {_settings.clientDataSettings.path}");
                return response;
            }

            if (_inv.itemCount > 0)
                _inv.items = new();

            for (var d = 0; d < _inv.itemCount; d++)
            {
                var _tempItem = new Item();

                _tempItem.slot = charData.ReadUInt8();

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

                _tempItem.id = charData.ReadUInt32(); //4   uint    item.RefItemID

                var itemFound = _allItems.TryGetValue((int)_tempItem.id, out var _foundItem);

                if (!itemFound)
                    continue;

                if (_foundItem == null)
                    continue;

                _tempItem.codeName128 = _foundItem.CodeName128;

                if (_foundItem.TypeID1 == 3)
                {
                    //ITEM_
                    if (_foundItem.TypeID2 == 1)
                    {
                        //ITEM_CH
                        //ITEM_EU
                        //AVATAR_
                        _tempItem.optLevel = charData.ReadUInt8();
                        _tempItem.variance = charData.ReadUInt64();
                        _tempItem.durability = charData.ReadUInt32();
                        _tempItem.magParamNum = charData.ReadUInt8();

                        for (var paramIndex = 0; paramIndex < _tempItem.magParamNum; paramIndex++)
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

                        Custom.WriteLine($"ITEM_ {_tempItem.slot} {_tempItem.id} {_tempItem.codeName128}");
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

                        Custom.WriteLine($"ITEM_ETC: {_tempItem.slot} {_tempItem.id} {_tempItem.codeName128}");

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

                _inv.items.Add(_tempItem);
            }

            foreach(var dd in _inv.items)
            {
                Custom.WriteLine($"{dd.ToString()}");
            }

            return response;
        }
    }
}
