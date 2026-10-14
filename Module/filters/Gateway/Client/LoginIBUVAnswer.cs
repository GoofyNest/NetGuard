namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x6323)]
    public sealed class LoginIBUVAnswer : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            //string code = packet.ReadAscii();
            return new PacketHandlingResult();
        }
    }
}