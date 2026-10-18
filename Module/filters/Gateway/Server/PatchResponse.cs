using NetGuard.Hot;

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA100)]
    public sealed class PatchResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            client.Gateway.SentPatchResponse = true;
            return new PacketHandlingResult();
        }
    }
}