/*
0x704C | Agent | Client -> Server
Name: InventoryItemUseRequest

// Variant: Chat.SendGlobalChatPacket
if (inventoryItem != null)
{
    byte inventoryItemSlot
    if (gameVersion > Vietnam)
    {
        int tid
        byte unknown_56 // value: 0
    }
    if (!(gameVersion > Vietnam))
    {
        ushort tid
    }
    if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
    {
        unicode message
    }
    else
    {
        ascii message
    }
}

// Variant: InventoryItem.Use
byte slot
if (gameVersion > Vietnam)
{
    int tid
}
if (!(gameVersion > Vietnam))
{
    ushort tid
}

// Variant: InventoryItem.UseTo
byte slot
if (gameVersion > Vietnam)
{
    int tid
}
if (!(gameVersion > Vietnam))
{
    ushort tid
}
byte destinationSlot
if (mapId > -1)
{
    int mapId
}

// Variant: InventoryItem.UseFor
byte slot
if (gameVersion > Vietnam)
{
    int tid
}
if (!(gameVersion > Vietnam))
{
    ushort tid
}
uint uniqueId

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

using NetGuard.Hot;
using NetGuard.Hot.Helpers;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x704C)]
    public sealed class InventoryItemUseRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            if (packet.RemainingRead() < 3)
                return new PacketHandlingResult { ResultType = PacketResultType.Block };
            packet.ReadUInt8(); // Slot
            ushort useType = packet.ReadUInt16();
            int type = PetTracking.ItemType(useType);
            var character = client.CurrentChar();

            if (character == null)
                return new PacketHandlingResult { ResultType = type != 0 ? PacketResultType.Block : PacketResultType.None };

            if (!PetTracking.TryUse(character, type))
            {
                ctx?.SendToClient(ChatPackets.CreateNotice(
                    !character.PetStateReady
                        ? "Pet data is not ready. Teleport or relog before summoning."
                        : "Only one attack pet and one grab pet may be summoned. Wait for the current pet action to finish."));
                return new PacketHandlingResult { ResultType = PacketResultType.Block };
            }
            return new PacketHandlingResult();
        }
    }
}
