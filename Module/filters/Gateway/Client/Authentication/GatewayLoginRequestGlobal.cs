/*
0x610A | Gateway | Client -> Server
Name: GatewayLoginRequestGlobal

// Variant: GatewayLoginRequestGlobal.Invoke
byte unknown_32 // locale
ascii unknown_33
ascii unknown_34
if ((packet.Opcode == 0x610A && gameVersion == Turkey) || gameVersion == VTC_Game || gameVersion == RuSro || gameVersion == Japanese || gameVersion == Taiwan)
{
    byte macAddress[6] // reassigned in case of manual login when auto is enabled
}
ushort shardId
if (gameVersion >= Chinese)
{
    byte unknown_48 // channel
}

// Variant: AutoLogin.SendLoginRequest
byte locale
if (gameVersion == RuSro)
{
    ascii login
    ascii password
}
if (!(gameVersion == RuSro))
{
    if (gameVersion == Japanese)
    {
        ascii empty
        ascii token
    }
    if (!(gameVersion == Japanese))
    {
        ascii username
        ascii password
    }
}
if (gameVersion == Turkey || gameVersion == VTC_Game || gameVersion == RuSro || gameVersion == Korean || gameVersion == Japanese || gameVersion == Taiwan)
{
    byte macAddress[Game.MacAddress.Length]
}
ushort id
if (opcode == 0x610A)
{
    byte channel
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

namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x610A)]
    public sealed class GatewayLoginRequestGlobal : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
