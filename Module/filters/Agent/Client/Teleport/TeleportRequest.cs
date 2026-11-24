/*
0x705A | Agent | Client -> Server
Name: TeleportRequest

// Variant: TeleportRequest.Invoke
uint teleporterUniqueId
byte teleportType
if (teleportType == TeleportType.Guide || teleportType == TeleportType.RUNTIME_PORTAL)
{
    byte operation
    // Stop parsing this variant.
}
uint destination
if (!SpawnManager.TryGetEntity<SpawnedBionic>(teleporterUniqueId, out var portal))
{
    // Stop parsing this variant.
}

// Variant: TeleportScriptCommand.ExecuteTeleport
uint uniqueId
byte unknown_75 // value: 0x02
uint destination

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
}

// Library/RSBot.Core/Objects/TeleportType.cs
enum TeleportType : byte
{
    Return = 1,
    Destination = 2,
    RUNTIME_PORTAL = 3,
    Guide = 5,
}

// Library/RSBot.Core/GameClientType.cs
enum GameClientType : byte
{
    jSRO = 0,
    Japanese_Old = 1,
    Thailand = 2,
    Vietnam = 3,
    Taiwan_Old = 4,
    Vietnam193 = 5,
    Vietnam274 = 6,
    Chinese_Old = 7,
    Chinese = 8,
    Global = 9,
    Turkey = 10,
    VTC_Game = 11,
    Taiwan = 12,
    Korean = 13,
    Japanese = 14,
    RuSro = 15,
    Rigid = 16,
}
*/

using NetGuard.Hot;
using NetGuard.Hot.Helpers;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x705A)]
    public sealed class TeleportRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            if (h.UseItem.AnyOnCooldown(out var which, out var remaining))
            {
                var notice = ChatPackets.CreateNotice(
                    $"{which} on cooldown ({remaining.TotalSeconds:F1}s)");

                response.ModifiedPackets.Add(new PacketList
                {
                    Packet = notice,
                    SecurityType = SecurityType.LocalSecurity,
                    SendImmediately = false,
                });
                
                return response;
            } 

            return response;
        }
    }
}