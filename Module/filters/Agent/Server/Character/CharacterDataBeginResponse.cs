/*
0x34A5 | Agent | Server -> Client
Name: CharacterDataBeginResponse

*/

using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x34A5)]
    public sealed class CharacterDataBeginResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                client.Host()!.Agent.CharData = null;
                if (client.CurrentChar()is { } character)
                    PetTracking.BeginSnapshot(character);
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x34A5 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
