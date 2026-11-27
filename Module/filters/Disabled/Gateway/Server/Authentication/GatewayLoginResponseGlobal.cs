/*
0xA10A | Gateway | Server -> Client
Name: GatewayLoginResponseGlobal

// Variant: GatewayLoginResponseGlobal.Invoke
byte unknown_36
if (unknown_36 == 0x01)
{
    if (gameVersion == Japanese)
    {
        uint unknown_45 // Token
        ascii unknown_46
        ushort unknown_47 // Port
        ascii unknown_48
        byte unknown_49 // Channel
    }
    if (gameVersion == Global && selectedAccount.Channel == 0x02)
    {
        uint unknown_58 // Token
        ascii unknown_59
        ushort unknown_60 // Port
        byte unknown_61 // Channel
        ascii unknown_62
    }
    // Stop parsing this variant.
}
byte code
switch (code): case 26:
{
    ushort count
    int timestamp
}

// Variant: GatewayLoginResponseHookOfficial.ReplacePacket (written payload)
byte result
uint token
ascii unknown_58
ushort port
if (packet.Remaining > 0)
{
    byte remainingPayload[remainingBytes]
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

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

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0xA10A)]
    public sealed class GatewayLoginResponseGlobal : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xA10A handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
