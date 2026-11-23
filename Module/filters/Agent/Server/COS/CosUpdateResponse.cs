using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30C9)]
    public sealed class CosUpdateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            if (packet.RemainingRead() >= 5 && client.CurrentChar() is { } character)
            {
                uint uniqueId = packet.ReadUInt32();
                byte status = packet.ReadUInt8();
                if (status == 1)
                {
                    int type = PetTracking.Destroy(character, uniqueId);
                    var state = client.Host()!.UseItem;
                    if (type == 1) state.AttackPetTime = DateTime.UtcNow;
                    else if (type == 2) state.GrabPetTime = DateTime.UtcNow;
                    else state.HorseTime = DateTime.UtcNow;
                }
            }
            return new PacketHandlingResult();
        }
    }
}
