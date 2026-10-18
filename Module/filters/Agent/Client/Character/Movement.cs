namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7021)]
    public sealed class PlayerMovement : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            
            return response;
        }
    }
}