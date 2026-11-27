/*
0x7116 | Agent | Client -> Server
Name: CosTerminateRequest

// Variant: Ability.Terminate
int uniqueId

// Variant: Growth.Terminate
int uniqueId

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7116)]
    public sealed class CosTerminateRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            uint UniqueID = packet.ReadUInt32();

            Custom.WriteLine($"0x7116 {UniqueID}");

            return new PacketHandlingResult();
        }
    }
}
