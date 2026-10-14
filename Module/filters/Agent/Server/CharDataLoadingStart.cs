namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x34A5)] // verify
    public sealed class CharDataLoadingStart : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            client.Agent.CharData = null!;

            return response;
        }
    }
}