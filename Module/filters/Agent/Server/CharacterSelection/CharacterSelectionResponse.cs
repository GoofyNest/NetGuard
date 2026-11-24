/*
0xB001 | Agent | Server -> Client
Name: CharacterSelectionResponse

*/

using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB001)]
    public sealed class CharacterSelectionResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var h = client.Host()!;
                var name = client.PlayerInfo.CurrentCharName;
                if (h.Agent.Current(name)is null)
                    h.Agent.Chars.Add(new CharState { Charname = name });
                client.Agent.SentJoinRequest = true;
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB001 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}