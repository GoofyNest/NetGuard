using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30C9)]
    public sealed class TerminatePet : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            packet.ReadUInt32(); // UniqueID

            byte status = packet.ReadUInt8();

            if(status == 0x01)
            {
                h.UseItem.HorseTime = DateTime.UtcNow;
                h.UseItem.AttackPetTime = DateTime.UtcNow;
                h.UseItem.GrabPetTime = DateTime.UtcNow;
            }

            return response;
        }
    }
}