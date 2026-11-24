/*
0x7461 | Agent | Client -> Server
Name: StallNetwork

// Payload fields are not identified by the available source.

*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7461)]
    public sealed class StallNetwork : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            
            return response;
        }
    }
}