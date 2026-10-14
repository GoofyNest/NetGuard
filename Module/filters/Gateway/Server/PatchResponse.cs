namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA100)] // SERVER_GATEWAY_PATCH_RESPONSE
    public sealed class PatchResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            client.Gateway.SentPatchResponse = true;
            return new PacketHandlingResult();
        }
    }
}