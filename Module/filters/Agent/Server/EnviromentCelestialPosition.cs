namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3020)] // verify
    public sealed class EnviromentCelestialPosition : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            var charInfo = client.PlayerInfo.CharInfo.Find(m => m.Charname == client.PlayerInfo.CurrentCharName);

            if (charInfo == null)
                return response;

            charInfo.IsVisible = false;

            return response;
        }
    }
}