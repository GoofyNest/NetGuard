/*
0xA101 | Gateway | Server -> Client
Name: GatewayServerListResponse

// Variant: GatewayServerListResponse.Invoke
repeat
{
    byte hasNext_35 // Stop repetition when !(hasNext_35 != 0)
    byte unknown_37
    ascii unknown_38
    byte hasNext_41 // Stop repetition when !(hasNext_41 == 1)
    ushort id
    if (gameVersion == Turkey || gameVersion == Global || gameVersion == VTC_Game || gameVersion == RuSro || gameVersion == Korean || gameVersion == Japanese || gameVersion == Taiwan)
    {
        unicode serverName
    }
    if (!(gameVersion == Turkey || gameVersion == Global || gameVersion == VTC_Game || gameVersion == RuSro || gameVersion == Korean || gameVersion == Japanese || gameVersion == Taiwan))
    {
        ascii serverName
    }
    if (gameVersion < Global)
    {
        ushort currentCapacity
        ushort maxCapacity
        byte status
    }
    if (!(gameVersion < Global))
    {
        byte status
        byte unknown_69
    }
    if (gameVersion == Vietnam)
    {
        byte unknown_98 // FarmId
    }
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Plugins/RSBot.General/PacketHandler/GatewayServerListResponse.cs
enum ServerStatusModern : int
{
    Full = 0,
    Crowded = 1,
    Populate = 2,
    Easy = 3,
    Check = 4,
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

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA101)]
    public sealed class GatewayServerListResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                client.Gateway.SentShardListResponse = true;
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xA101 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}