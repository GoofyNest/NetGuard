namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB007)]
    public sealed class CharacterSelectionResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            client.Agent.InCharSelectionScreen = true;

            return response;
        }
    }
}