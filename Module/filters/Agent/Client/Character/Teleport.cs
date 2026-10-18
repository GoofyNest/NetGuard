using NetGuard.Hot;
using NetGuard.Hot.Helpers;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x705A)]
    public sealed class PlayerTeleport : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            if (h.UseItem.AnyOnCooldown(out var which, out var remaining))
            {
                var notice = ChatPackets.CreateNotice(
                    $"{which} on cooldown ({remaining.TotalSeconds:F1}s)");

                response.ModifiedPackets.Add(new PacketList
                {
                    Packet = notice,
                    SecurityType = SecurityType.LocalSecurity,
                    SendImmediately = false,
                });
                
                return response;
            } 

            return response;
        }
    }
}