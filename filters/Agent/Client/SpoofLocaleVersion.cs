namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x3014)]
    public sealed class SpoofLocaleVersion : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            var modified = new Packet((ushort)0x3012); // Game.Ready1

            response.ModifiedPackets.Add(new PacketList { Packet = modified });

            return response;
        }
    }
}