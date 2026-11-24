/*
0x7151 | Agent | Client -> Server
Name: StoneFuseRequestHandler

// Variant: StoneFuseRequestHandler.Invoke
byte action
if (action != AlchemyAction.Fuse)
{
    // Stop parsing this variant.
}
byte type
if (type == AlchemyType.SocketInsert)
{
    byte item // Target item
    byte socketItem // Target item
    // Stop parsing this variant.
}
byte slotsCount
byte slots[slotsCount]

// Variant: AlchemyManager.TryFuseMagicStone
byte fuse // Fuse
byte magicStone // MagicStone
byte slotCount // Slot count
byte itemSlot
byte magicStoneSlot

// Variant: AlchemyManager.TryFuseAttributeStone
byte fuse // Fuse
byte attributeStone // MagicStone
byte slotCount // Slot count
byte itemSlot
byte attributeStoneSlot

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Client/ReferenceObjects/AlchemyAction.cs
enum AlchemyAction : byte
{
    Cancel = 1,
    Fuse = 2,
    SocketCreate = 3,
    SocketRemove = 4,
}

// Library/RSBot.Core/Client/ReferenceObjects/AlchemyType.cs
enum AlchemyType : byte
{
    Disjoin = 1,
    Manufacture = 2,
    Elixir = 3,
    MagicStone = 4,
    AttributeStone = 5,
    AdvancedElixirOrSocketCreate = 8,
    SocketInsert = 9,
    SocketRemove = 10,
    EnhancerElixir = 19,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7151)]
    public sealed class StoneFuseRequestHandler : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
