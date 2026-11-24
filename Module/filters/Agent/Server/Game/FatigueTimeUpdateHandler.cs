/*
0x3123 | Agent | Server -> Client
Name: FatigueTimeUpdateHandler

// Variant: FatigueTimeUpdateHandler.Invoke
if (gameVersion == VTC_Game)
{
    uint serviceFatigueSecondsSpent
    byte unknown_19 // serviceFatigue type or color of bar (0x80 - 100%, 0x00 - 50%)
    byte unknown_20 // serviceFatigue???
    byte unknown_21 // 0x04 and >0x0F changes notice on hover
    byte unknown_22[2] // serviceFatigue???
    byte serviceFatigueGreenHours
    byte unknown_24 // serviceFatigueTotalHours (green+yellow)
    byte unknown_25 // serviceFatigue???
    byte unknown_26 // serviceFatigue???
    byte unknown_28[4] // ???
}
if (!(gameVersion == VTC_Game))
{
    uint unknown_40 // serviceFatigueSecondsSpent
    byte unknown_41 // serviceFatigue type or color of bar (0x80 - 100%, 0x00 - 50%)
    byte unknown_42 // serviceFatigue???
    byte unknown_43 // 0x04 and >0x0F changes notice on hover
    byte unknown_44[2] // serviceFatigue???
    byte unknown_45 // serviceFatigueYellowHours
    byte unknown_46 // serviceFatigueTotalHours (yellow+red)
    byte unknown_47 // serviceFatigue???
    byte unknown_48 // serviceFatigue???
    byte unknown_50[2] // ???
    uint shardFatigueSecondsSpent
    byte unknown_53 // shardFatigue type or color of bar (0x80 - 100%, 0x00 - 50%)
    byte unknown_54 // shardFatigue???
    byte unknown_55 // 0x04 and >0x0F changes notice on hover
    byte unknown_56[2] // shardFatigue???
    byte shardFatigueBlueHours
    byte unknown_58 // shardFatigueTotalHours (blue+purple)
    byte unknown_59 // shardFatigue???
    byte unknown_60 // shardFatigue???
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

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3123)]
    public sealed class FatigueTimeUpdateHandler : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3123 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
