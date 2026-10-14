namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB001)] // verify against Agent/Server/PacketHandler.cs
    public sealed class CharacterJoin : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            if (client.PlayerInfo.CharInfo == null)
                client.PlayerInfo.CharInfo = [];

            var charInfo = client.PlayerInfo.CharInfo;

            var index = charInfo.FindIndex(m => m.Charname == client.PlayerInfo.CurrentCharName);

            if (index == -1)
                charInfo.Add(new CharacterInformation() { Charname = client.PlayerInfo.CurrentCharName });

            client.Agent.SentJoinRequest = true;

            return response;
        }
    }
}