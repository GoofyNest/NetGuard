/*
0x3020 | Agent | Server -> Client
Name: EnviromentCelestialPosition

// Current NetGuard parsed fields (may cover only part of the payload):
uint uniqueID

*/

using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3020)]
    public sealed class EnviromentCelestialPosition : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var charInfo = client.CurrentChar();
                if (charInfo == null)
                    return new PacketHandlingResult();
                charInfo.IsVisible = false;
                try
                {
                    var UniqueID = packet.ReadUInt32();
                    charInfo.UniqueID = UniqueID;
                    Custom.WriteLine($"[S->C] 0x3020 UniqueID: {UniqueID}");
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[S->C] 0x3020 {ex}", ConsoleColor.Red);
                }
        
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3020 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}