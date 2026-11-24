/*
0x6100 | Gateway | Client -> Server
Name: PatchRequest

// Variant: GlobalIdentificationRequest.Invoke
if (serviceName == "GatewayServer")
{
    byte locale
    ascii unknown_41
    uint version
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Network/PacketDestination.cs
enum PacketDestination : int
{
    Server = 1,
    Client = 2,
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

using Module.Networking;
using Module.PacketHandler;
using Module.PacketHandler.Filtering;
using Module.Services;
using SilkroadSecurityAPI;

namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x6100)]
    public sealed class PatchRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            byte contentID = packet.ReadUInt8();
            string ModuleName = packet.ReadAscii();
            uint version = packet.ReadUInt32();

            Custom.WriteLine($"contentID {contentID}");
            Custom.WriteLine($"ModuleName {ModuleName}");
            Custom.WriteLine($"version {version}");

            return response;
        }
    }
}