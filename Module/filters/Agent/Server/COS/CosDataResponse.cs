using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30C8)]
    public sealed class CosDataResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            if (packet.RemainingRead() >= 8 && client.CurrentChar() is { } character)
            {
                uint uniqueId = packet.ReadUInt32();
                uint objectId = packet.ReadUInt32();
                PetTracking.Spawn(character, uniqueId, objectId);
            }
            return new PacketHandlingResult();
        }
    }
}
