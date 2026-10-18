namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7158)]
    public sealed class HotkeyUpdate : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            return response;
        }
    }
}