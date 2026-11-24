/*
0x6103 | Agent | Client -> Server
Name: AgentLoginRequestHook

// Variant: AgentLoginRequestHook.ReplacePacket (written payload)
uint token
if (gameVersion == RuSro)
{
    ascii login
    ascii passwordHash
}
if (!(gameVersion == RuSro))
{
    if (gameVersion == Japanese)
    {
        ascii login
        ascii tokenHash
    }
    if (!(gameVersion == Japanese))
    {
        if (gameVersion == Global && selectedAccount.Channel == 0x02)
        {
            ascii login
        }
        if (!(gameVersion == Global && selectedAccount.Channel == 0x02))
        {
            ascii username
        }
        if (gameVersion == Turkey || gameVersion == VTC_Game || gameVersion == Global || gameVersion == Korean || gameVersion == Taiwan)
        {
            ascii unknown_72
        }
        if (!(gameVersion == Turkey || gameVersion == VTC_Game || gameVersion == Global || gameVersion == Korean || gameVersion == Taiwan))
        {
            ascii password
        }
    }
}
byte locale
byte macAddress[Game.MacAddress.Length]

// Variant: GlobalIdentificationRequest.Invoke
if (!(serviceName == "GatewayServer"))
{
    if (serviceName == "AgentServer")
    {
        uint token
        if (gameVersion == RuSro)
        {
            ascii login
            ascii passwordHash
        }
        if (!(gameVersion == RuSro))
        {
            if (gameVersion == Japanese)
            {
                ascii login
                ascii tokenHash
            }
            if (!(gameVersion == Japanese))
            {
                if (gameVersion == Global && selectedAccount.Channel == 0x02)
                {
                    ascii login
                }
                if (!(gameVersion == Global && selectedAccount.Channel == 0x02))
                {
                    ascii username
                }
                if (gameVersion == Turkey || gameVersion == VTC_Game || gameVersion == Global || gameVersion == Korean || gameVersion == Taiwan)
                {
                    ascii unknown_87
                }
                if (!(gameVersion == Turkey || gameVersion == VTC_Game || gameVersion == Global || gameVersion == Korean || gameVersion == Taiwan))
                {
                    ascii password
                }
            }
        }
        byte locale
        byte macAddress[Game.MacAddress.Length]
    }
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

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x6103)]
    public sealed class AgentLoginRequestHook : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            packet.ReadUInt32(); // token
            client.PlayerInfo.Username = packet.ReadAscii();
            client.PlayerInfo.Password = packet.ReadAscii();
            packet.ReadUInt8(); // operation type

            var mac = packet.ReadUInt8Array(6);
            client.PlayerInfo.Mac = BitConverter.ToString(mac);

            return response;
        }
    }
}