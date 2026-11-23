using NetGuard.Hot;
namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x305C)]
    public sealed class AnimationUseItem : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            //return new PacketHandlingResult();

            var h = client.Host()!;
            var response = new PacketHandlingResult();

            var character = client.CurrentChar();

            try {
                var unk = packet.ReadUInt32();
                var unk1 = packet.ReadUInt32();

                Custom.WriteLine($"[S->C] 0x305C unk: {unk} unk1: {unk1}");
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[S->C] 0x305C {ex}", ConsoleColor.Red);
            }

            return response;
        }
    }
}