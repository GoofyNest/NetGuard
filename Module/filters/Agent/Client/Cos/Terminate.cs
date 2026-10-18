namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x70C6)]
    public sealed class CosTerminate : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();



            return response;
        }
    }
}