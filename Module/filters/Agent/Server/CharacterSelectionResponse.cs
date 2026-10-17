namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB007)]
    public sealed class CharacterSelectionResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            client.Agent.InCharSelectionScreen = true;
            return new PacketHandlingResult();
        }
    }
}