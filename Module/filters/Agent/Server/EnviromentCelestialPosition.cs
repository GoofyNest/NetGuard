using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3020)]
    public sealed class EnviromentCelestialPosition : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var charInfo = client.CurrentChar();
            if (charInfo == null) return new PacketHandlingResult();
            charInfo.IsVisible = false;
            return new PacketHandlingResult();
        }
    }
}