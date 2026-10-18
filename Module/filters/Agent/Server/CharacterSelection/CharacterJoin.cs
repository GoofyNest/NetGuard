using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB001)]
    public sealed class CharacterJoin : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var name = client.PlayerInfo.CurrentCharName;

            if (h.Agent.Current(name) is null)
                h.Agent.Chars.Add(new CharState { Charname = name });

            client.Agent.SentJoinRequest = true;
            return new PacketHandlingResult();
        }
    }
}