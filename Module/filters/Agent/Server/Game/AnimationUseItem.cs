/*
0x305C | Agent | Server -> Client
Name: AnimationUseItem

// Current NetGuard parsed fields (may cover only part of the payload):
uint unk
uint unk1

*/

using NetGuard.Hot;
namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x305C)]
    public sealed class AnimationUseItem : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                //return new PacketHandlingResult();
                var h = client.Host()!;
                var response = new PacketHandlingResult();
                var character = client.CurrentChar();
                try
                {
                    var unk = packet.ReadUInt32();
                    var unk1 = packet.ReadUInt32();
                    Custom.WriteLine($"[S->C] 0x305C unk: {unk} unk1: {unk1}");
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[S->C] 0x305C {ex}", ConsoleColor.Red);
                }
        
                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x305C handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}