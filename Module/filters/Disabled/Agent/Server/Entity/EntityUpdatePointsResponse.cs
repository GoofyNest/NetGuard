/*
0x304E | Agent | Server -> Client
Name: EntityUpdatePointsResponse

// Variant: PlayerUpdatePointsResponse.Invoke
byte type
switch (type): case 1:
{
    ulong gold
}
switch (type): case 2:
{
    uint skillPoints
}
switch (type): case 4:
{
    byte berzerkPoints
}

*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x304E)]
    public sealed class EntityUpdatePointsResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
