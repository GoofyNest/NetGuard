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
            return new PacketHandlingResult();
        }
    }
}