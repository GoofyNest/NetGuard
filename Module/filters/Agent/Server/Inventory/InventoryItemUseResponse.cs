/*
0xB04C | Agent | Server -> Client
Name: InventoryItemUseResponse

// Variant: InventoryItemUseResponse.Invoke
byte unknown_29
if (unknown_29 != 0x01)
{
    // Stop parsing this variant.
}
byte sourceSlot
ushort newAmount

// Response fields checked by InventoryItem.Use
byte predicate

// Response fields checked by InventoryItem.UseTo
byte predicate

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/AwaitCallback.cs
enum AwaitCallbackResult : int
{
    ConditionFailed = 0,
    Success = 1,
    Fail = 2,
}

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}

// Library/RSBot.Core/GameClientType.cs
enum GameClientType : byte
{
    jSRO = 0,
    Japanese_Old = 1,
    Thailand = 2,
    Vietnam = 3,
    Taiwan_Old = 4,
    Vietnam193 = 5,
    Vietnam274 = 6,
    Chinese_Old = 7,
    Chinese = 8,
    Global = 9,
    Turkey = 10,
    VTC_Game = 11,
    Taiwan = 12,
    Korean = 13,
    Japanese = 14,
    RuSro = 15,
    Rigid = 16,
}
*/

using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB04C)]
    public sealed class InventoryItemUseResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var response = new PacketHandlingResult();
                try
                {
                    var h = client.Host()!;
                    byte status = packet.ReadUInt8();
                    var character = client.CurrentChar();
                    
                    if (character != null)
                        PetTracking.UseResult(character, status == 0x01);
                        
                    if (status == 0x01)
                    {
                        byte Slot = packet.ReadUInt8();
                        ushort unk = packet.ReadUInt16();
                        ushort ItemType = packet.ReadUInt16();
                        if (character != null)
                            character.LastUseItem = ItemType;
                        switch (ItemType)
                        {
                            /*
                                    Rideable horse
                                    Trade pets
                                */
                            case 0x11ED: // Item mall
                            case 0x11EC: // Normal
                            {
                                h.UseItem.HorseTime = DateTime.UtcNow;
                            }
        
                                break;
                            /*
                                    Grab pets
                                */
                            case 0x10CD:
                            {
                                h.UseItem.GrabPetTime = DateTime.UtcNow;
                            }
        
                                break;
                            /*
                                    Attack pets
                                */
                            case 0x08CD: // Item mall
                            case 0x08CC: // Normal
                            {
                                h.UseItem.AttackPetTime = DateTime.UtcNow;
                            }
        
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[S->C] 0xB04C {ex}", ConsoleColor.Red);
                }
        
                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB04C handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
