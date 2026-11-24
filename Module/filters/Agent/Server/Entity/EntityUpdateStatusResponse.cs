/*
0x3057 | Agent | Server -> Client
Name: EntityUpdateStatusResponse

// Variant: EntityUpdateStatusResponse.Invoke
uint uniqueId
ushort updateSource // UpdateSource
byte updateFlag
if (uniqueId == Game.Player.UniqueId)
{
    if ((updateFlag & EntityUpdateStatusFlag.HP) == EntityUpdateStatusFlag.HP)
    {
        int health
    }
    if ((updateFlag & EntityUpdateStatusFlag.MP) == EntityUpdateStatusFlag.MP)
    {
        int mana
    }
    if ((updateFlag & EntityUpdateStatusFlag.BadEffect) == EntityUpdateStatusFlag.BadEffect)
    {
        uint effectCurrent
        repeat for each effectValue in Enum.GetValues(typeof(BadEffect))
        {
            if ((effectCurrent & effectValue) > BadEffect.Zombie)
            {
                byte effectLevel // EffectLevel
            }
        }
    }
}
if (!(uniqueId == Game.Player.UniqueId))
{
    if (Game.Player.Growth?.UniqueId == uniqueId)
    {
        if ((updateFlag & EntityUpdateStatusFlag.HP) == EntityUpdateStatusFlag.HP)
        {
            int health
        }
        if ((updateFlag & EntityUpdateStatusFlag.MP) == EntityUpdateStatusFlag.MP)
        {
            uint unknown_108
        }
        if ((updateFlag & EntityUpdateStatusFlag.BadEffect) == EntityUpdateStatusFlag.BadEffect)
        {
            uint effectCurrent
            repeat for each effectValue in Enum.GetValues(typeof(BadEffect))
            {
                if ((effectCurrent & effectValue) > BadEffect.Zombie)
                {
                    byte effectLevel // EffectLevel
                }
            }
        }
    }
    if (!(Game.Player.Growth?.UniqueId == uniqueId))
    {
        if (Game.Player.Fellow?.UniqueId == uniqueId)
        {
            if ((updateFlag & EntityUpdateStatusFlag.HP) == EntityUpdateStatusFlag.HP)
            {
                int health
            }
            if ((updateFlag & EntityUpdateStatusFlag.MP) == EntityUpdateStatusFlag.MP)
            {
                uint unknown_153
            }
            if (updateFlag == EntityUpdateStatusFlag.Fellow)
            {
                int satiety
            }
            if ((updateFlag & EntityUpdateStatusFlag.BadEffect) == EntityUpdateStatusFlag.BadEffect)
            {
                uint effectCurrent
                repeat for each effectValue in Enum.GetValues(typeof(BadEffect))
                {
                    if ((effectCurrent & effectValue) > BadEffect.Zombie)
                    {
                        byte effectLevel // EffectLevel
                    }
                }
            }
        }
        if (!(Game.Player.Fellow?.UniqueId == uniqueId))
        {
            if (Game.Player.Transport?.UniqueId == uniqueId)
            {
                if ((updateFlag & EntityUpdateStatusFlag.HP) == EntityUpdateStatusFlag.HP)
                {
                    int health
                }
                if ((updateFlag & EntityUpdateStatusFlag.MP) == EntityUpdateStatusFlag.MP)
                {
                    uint unknown_205
                }
                if ((updateFlag & EntityUpdateStatusFlag.BadEffect) == EntityUpdateStatusFlag.BadEffect)
                {
                    uint effectCurrent
                    repeat for each effectValue in Enum.GetValues(typeof(BadEffect))
                    {
                        if ((effectCurrent & effectValue) > BadEffect.Zombie)
                        {
                            byte effectLevel // EffectLevel
                        }
                    }
                }
            }
            if (!(Game.Player.Transport?.UniqueId == uniqueId))
            {
                if (Game.Player.JobTransport?.UniqueId == uniqueId)
                {
                    if ((updateFlag & EntityUpdateStatusFlag.HP) == EntityUpdateStatusFlag.HP)
                    {
                        int health
                    }
                    if ((updateFlag & EntityUpdateStatusFlag.MP) == EntityUpdateStatusFlag.MP)
                    {
                        uint unknown_250
                    }
                    if ((updateFlag & EntityUpdateStatusFlag.BadEffect) == EntityUpdateStatusFlag.BadEffect)
                    {
                        uint effectCurrent
                        repeat for each effectValue in Enum.GetValues(typeof(BadEffect))
                        {
                            if ((effectCurrent & effectValue) > BadEffect.Zombie)
                            {
                                byte effectLevel // EffectLevel
                            }
                        }
                    }
                }
                if (!(Game.Player.JobTransport?.UniqueId == uniqueId))
                {
                    if (SpawnManager.TryGetEntity<SpawnedBionic>(uniqueId, out var entity))
                    {
                        if ((updateFlag & EntityUpdateStatusFlag.HP) == EntityUpdateStatusFlag.HP)
                        {
                            int health
                        }
                        if ((updateFlag & EntityUpdateStatusFlag.MP) == EntityUpdateStatusFlag.MP)
                        {
                            uint unknown_303
                        }
                    }
                }
            }
        }
    }
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/BadEffect.cs
[Flags]
enum BadEffect : uint
{
    None = 0,
    Frozen = 1,
    Frostbitten = 2,
    Shocked = 4,
    Burnt = 8,
    Poisoned = 16,
    Zombie = 32,
    Sleep = 64,
    Bind = 128,
    Dull = 256,
    Fear = 512,
    ShortSighted = 1024,
    Bleed = 2048,
    Petrify = 4096,
    Darkness = 8192,
    Stunned = 16384,
    Disease = 32768,
    Confusion = 65536,
    Decay = 131072,
    Weak = 262144,
    Impotent = 524288,
    Division = 1048576,
    Panic = 2097152,
    Combustion = 4194304,
    Bit23 = 8388608,
    Hidden = 16777216,
    Bit25 = 33554432,
    Bit26 = 67108864,
    Bit27 = 134217728,
    Bit28 = 268435456,
    Bit29 = 536870912,
    Bit30 = 1073741824,
    Bit31 = 2147483648,
}

// Library/RSBot.Core/Network/Handler/Agent/Entity/EntityUpdateStatusFlag.cs
[Flags]
enum EntityUpdateStatusFlag : byte
{
    None = 0,
    HP = 1,
    MP = 2,
    HPMP = 3,
    BadEffect = 4,
    Fellow = 13,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3057)]
    public sealed class EntityUpdateStatusResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3057 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
