/*
0x3015 | Agent | Server -> Client
Name: EntitySingleSpawnResponse

// Variant: EntitySingleSpawnResponse.Invoke
uint refObjId
if (refObjId == uint.MaxValue)
{
    if (gameVersion >= Chinese)
    {
        uint unknown_30
    }
    if (!(gameVersion >= Chinese))
    {
        ushort unknown_32
    }
    uint skillId
    uint uniqueId
    ushort region
    float xOffset
    float zOffset
    float yOffset
    short angle
    // Stop parsing this variant.
}
if (refObjId == 0xfffffffe)
{
    uint unknown_215
    uint unknown_216
}
if (obj == null)
{
    // Stop parsing this variant.
}
switch (obj.TypeID1): case 1:
{
    switch (obj.TypeID2): case 1:
    {
        byte scale
        if (gameVersion > Japanese_Old)
        {
            byte hwanLevel
        }
        if (gameVersion > Thailand)
        {
            byte pvpCape
        }
        byte autoInverstExp
        if (gameVersion > Chinese)
        {
            byte unknown_211 // Archievement Title
        }
        if (gameVersion == Taiwan)
        {
            uint unknown_214
        }
        byte inventorySize
        byte itemCount
        repeat (var i = 0; i < itemCount; i++)
        {
            uint itemId
            if (itemObj == null)
            {
                byte unknown_228
            }
            if (itemObj.IsEquip)
            {
                byte unknown_238 // Item object and the "+" value as value
            }
        }
        if (gameVersion >= Thailand)
        {
            byte avatarInventorySize
            byte itemCount
            repeat (var i = 0; i < itemCount; i++)
            {
                uint itemId
                if (itemObj == null)
                {
                    byte unknown_253
                }
                byte unknown_258 // Item object and the "+" value as value
            }
        }
        byte hasMask // boolean: 0=false, nonzero=true
        if (hasMask)
        {
            uint maskId
            if (maskObj == null)
            {
                // Stop parsing this variant.
            }
            if (maskObj.TypeID1 == Record.TypeID1 || maskObj.TypeID2 == Record.TypeID2)
            {
                byte scale
                byte itemCount
                repeat (var i = 0; i < itemCount; i++)
                {
                    uint unknown_279 // item id
                }
            }
        }
        uint uniqueId
        ushort region
        float xOffset
        float zOffset
        float yOffset
        short angle
        byte hasDestination // boolean: 0=false, nonzero=true
        byte type
        if (result.HasDestination)
        {
            ushort region
            if (!position.Region.IsDungeon)
            {
                short xOffset
                short zOffset
                short yOffset
            }
            if (!(!position.Region.IsDungeon))
            {
                int xOffset
                int zOffset
                int yOffset
            }
        }
        if (!(result.HasDestination))
        {
            byte unknown_114 // 0 = Spinning, 1 = Sky-/Key-walking
            short angle
        }
        byte lifeState
        if (gameVersion == jSRO || gameVersion > Thailand)
        {
            byte unknown_128 // unkByte0
        }
        byte motionState
        if (gameVersion != jSRO)
        {
            byte bodyState
        }
        if (gameVersion > Vietnam193)
        {
            byte unknown_138 // hasRedArrowEffect
        }
        float walkSpeed
        float runSpeed
        float berzerkSpeed
        byte buffCount
        repeat (var i = 0; i < buffCount; i++)
        {
            uint id
            uint token
            if (buff.Record.Params.Contains(1701213281))
            {
                byte unknown_159 // IsCreator // boolean: 0=false, nonzero=true
            }
        }
        ascii name
        byte job
        if (gameVersion >= Chinese_Old && WearsJobSuite)
        {
            if (WearsJobSuite)
            {
                byte unknown_291 // JobRank
                byte jobLevel
                byte unknown_293 // ??
            }
        }
        if (gameVersion < Chinese_Old)
        {
            byte jobLevel
            byte pvpState
        }
        byte onTransport // boolean: 0=false, nonzero=true
        byte inCombat // boolean: 0=false, nonzero=true
        if (OnTransport)
        {
            uint transportUniqueId
        }
        byte scrollMode
        byte interactMode
        if (gameVersion < Chinese_Old)
        {
            byte unknown_312 // unkByte4
        }
        ascii guildName
        if (!WearsJobSuite)
        {
            uint id
            ascii nickname
            uint lastCrestRev
            uint id
            uint lastCrestRev
            if (gameVersion >= Thailand)
            {
                byte isFriendly // boolean: 0=false, nonzero=true
                byte fortSiegeAuthority
            }
        }
        if (gameVersion >= Chinese && InteractMode == InteractMode.P2N_TALK2)
        {
            if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
            {
                unicode name
            }
            else
            {
                ascii name
            }
            uint decorationId
        }
        if (!(gameVersion >= Chinese && InteractMode == InteractMode.P2N_TALK2))
        {
            if (gameVersion < Chinese && InteractMode == InteractMode.P2N_TALK)
            {
                if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
                {
                    unicode name
                }
                else
                {
                    ascii name
                }
                uint decorationId
            }
        }
        if (gameVersion >= Chinese)
        {
            byte unknown_333[9]
        }
        byte unknown_335 // Equipment Cooldown
        byte pKFlag // PKFlag
        if (gameVersion >= Chinese && gameVersion < Rigid)
        {
            byte unknown_340 // 0xFF what flag?
        }
    }
    switch (obj.TypeID2): case 2:
    {
        switch (obj.TypeID3): case 1:
        {
            uint uniqueId
            ushort region
            float xOffset
            float zOffset
            float yOffset
            short angle
            byte hasDestination // boolean: 0=false, nonzero=true
            byte type
            if (result.HasDestination)
            {
                ushort region
                if (!position.Region.IsDungeon)
                {
                    short xOffset
                    short zOffset
                    short yOffset
                }
                if (!(!position.Region.IsDungeon))
                {
                    int xOffset
                    int zOffset
                    int yOffset
                }
            }
            if (!(result.HasDestination))
            {
                byte unknown_114 // 0 = Spinning, 1 = Sky-/Key-walking
                short angle
            }
            byte lifeState
            if (gameVersion == jSRO || gameVersion > Thailand)
            {
                byte unknown_128 // unkByte0
            }
            byte motionState
            if (gameVersion != jSRO)
            {
                byte bodyState
            }
            if (gameVersion > Vietnam193)
            {
                byte unknown_138 // hasRedArrowEffect
            }
            float walkSpeed
            float runSpeed
            float berzerkSpeed
            byte buffCount
            repeat (var i = 0; i < buffCount; i++)
            {
                uint id
                uint token
                if (buff.Record.Params.Contains(1701213281))
                {
                    byte unknown_159 // IsCreator // boolean: 0=false, nonzero=true
                }
            }
            byte flag
            if ((Flag & 2) > 0)
            {
                if (gameVersion > Thailand)
                {
                    byte count
                }
                byte options[count]
            }
            if (Flag == 6)
            {
                byte unknown_53 // maybe
                if (unknown_53 == 1)
                {
                    uint unknown_54
                }
            }
            byte rarity
            if (gameVersion > Chinese && gameVersion != Japanese)
            {
                uint unknown_78
            }
            if (Record.TypeID4 == 2 || Record.TypeID4 == 3)
            {
                byte unknown_81 // Appeareance
            }
        }
        switch (obj.TypeID3): case 3:
        {
            uint uniqueId
            ushort region
            float xOffset
            float zOffset
            float yOffset
            short angle
            byte hasDestination // boolean: 0=false, nonzero=true
            byte type
            if (result.HasDestination)
            {
                ushort region
                if (!position.Region.IsDungeon)
                {
                    short xOffset
                    short zOffset
                    short yOffset
                }
                if (!(!position.Region.IsDungeon))
                {
                    int xOffset
                    int zOffset
                    int yOffset
                }
            }
            if (!(result.HasDestination))
            {
                byte unknown_114 // 0 = Spinning, 1 = Sky-/Key-walking
                short angle
            }
            byte lifeState
            if (gameVersion == jSRO || gameVersion > Thailand)
            {
                byte unknown_128 // unkByte0
            }
            byte motionState
            if (gameVersion != jSRO)
            {
                byte bodyState
            }
            if (gameVersion > Vietnam193)
            {
                byte unknown_138 // hasRedArrowEffect
            }
            float walkSpeed
            float runSpeed
            float berzerkSpeed
            byte buffCount
            repeat (var i = 0; i < buffCount; i++)
            {
                uint id
                uint token
                if (buff.Record.Params.Contains(1701213281))
                {
                    byte unknown_159 // IsCreator // boolean: 0=false, nonzero=true
                }
            }
            byte flag
            if ((Flag & 2) > 0)
            {
                if (gameVersion > Thailand)
                {
                    byte count
                }
                byte options[count]
            }
            if (Flag == 6)
            {
                byte unknown_53 // maybe
                if (unknown_53 == 1)
                {
                    uint unknown_54
                }
            }
            if (refObj.TypeID4 == 2 //NPC_COS_TRANSPORT || refObj.TypeID4 == 3 //NPC_COS_P_GROWTH || refObj.TypeID4 == 4 //NPC_COS_P_ABILITY || refObj.TypeID4 == 5 //NPC_COS_GUILD || refObj.TypeID4 == 6 //NPC_COS_CAPTURED || refObj.TypeID4 == 7 //NPC_COS_QUEST || refObj.TypeID4 == 8 //NPC_COS_QUEST || refObj.TypeID4 == 9)
            {
                if (refObj.TypeID4 == 3 || // NPC_COS_P_GROWTH refObj.TypeID4 == 4 || //NPC_COS_P_ABILITY refObj.TypeID4 == 9)
                {
                    ascii name
                }
                if (!(refObj.TypeID4 == 3 || // NPC_COS_P_GROWTH refObj.TypeID4 == 4 || //NPC_COS_P_ABILITY refObj.TypeID4 == 9))
                {
                    if (refObj.TypeID4 == 5)
                    {
                        ascii guildName
                    }
                }
                if (refObj.TypeID4 == 2 //NPC_COS_TRASNPORT || refObj.TypeID4 == 3 //NPC_COS_P_GROWTH || refObj.TypeID4 == 4 //NPC_COS_P_ABILITY || refObj.TypeID4 == 5 //NPC_COS_GUILD || refObj.TypeID4 == 6 //NPC_COS_CAPTURED || refObj.TypeID4 == 9)
                {
                    ascii ownerName
                    if (refObj.TypeID4 == 2 //NPC_COS_TRASNPORT || refObj.TypeID4 == 3 //NPC_COS_P_GROWTH || refObj.TypeID4 == 4 //NPC_COS_ABILITY || refObj.TypeID4 == 5 //NPC_COS_GUILD || refObj.TypeID4 == 9)
                    {
                        byte unknown_94 // Owner job type
                        if (refObj.TypeID4 == 2 //NPC_COS_TRASNPORT || refObj.TypeID4 == 3 //NPC_COS_P_GROWTH || refObj.TypeID4 == 5 //NPC_COS_GUILD || refObj.TypeID4 == 9)
                        {
                            byte unknown_103 // Owner PVP state
                            if (refObj.TypeID4 == 5)
                            {
                                uint unknown_106 // OwnerRefID guild foo
                            }
                        }
                    }
                }
                uint ownerUniqueId
                if (refObj.TypeID4 == 9)
                {
                    byte unknown_113 // ???
                }
            }
        }
        switch (obj.TypeID3): case 5:
        {
            uint hP
            uint refEventStructID
            ushort currentState
            uint uniqueId
            ushort region
            float xOffset
            float zOffset
            float yOffset
            short angle
            byte hasDestination // boolean: 0=false, nonzero=true
            byte type
            if (result.HasDestination)
            {
                ushort region
                if (!position.Region.IsDungeon)
                {
                    short xOffset
                    short zOffset
                    short yOffset
                }
                if (!(!position.Region.IsDungeon))
                {
                    int xOffset
                    int zOffset
                    int yOffset
                }
            }
            if (!(result.HasDestination))
            {
                byte unknown_114 // 0 = Spinning, 1 = Sky-/Key-walking
                short angle
            }
            byte lifeState
            if (gameVersion == jSRO || gameVersion > Thailand)
            {
                byte unknown_128 // unkByte0
            }
            byte motionState
            if (gameVersion != jSRO)
            {
                byte bodyState
            }
            if (gameVersion > Vietnam193)
            {
                byte unknown_138 // hasRedArrowEffect
            }
            float walkSpeed
            float runSpeed
            float berzerkSpeed
            byte buffCount
            repeat (var i = 0; i < buffCount; i++)
            {
                uint id
                uint token
                if (buff.Record.Params.Contains(1701213281))
                {
                    byte unknown_159 // IsCreator // boolean: 0=false, nonzero=true
                }
            }
            byte flag
            if ((Flag & 2) > 0)
            {
                if (gameVersion > Thailand)
                {
                    byte count
                }
                byte options[count]
            }
            if (Flag == 6)
            {
                byte unknown_53 // maybe
                if (unknown_53 == 1)
                {
                    uint unknown_54
                }
            }
            uint guildId
            ascii guildName
        }
        switch (obj.TypeID3): default:
        {
            uint uniqueId
            ushort region
            float xOffset
            float zOffset
            float yOffset
            short angle
            byte hasDestination // boolean: 0=false, nonzero=true
            byte type
            if (result.HasDestination)
            {
                ushort region
                if (!position.Region.IsDungeon)
                {
                    short xOffset
                    short zOffset
                    short yOffset
                }
                if (!(!position.Region.IsDungeon))
                {
                    int xOffset
                    int zOffset
                    int yOffset
                }
            }
            if (!(result.HasDestination))
            {
                byte unknown_114 // 0 = Spinning, 1 = Sky-/Key-walking
                short angle
            }
            byte lifeState
            if (gameVersion == jSRO || gameVersion > Thailand)
            {
                byte unknown_128 // unkByte0
            }
            byte motionState
            if (gameVersion != jSRO)
            {
                byte bodyState
            }
            if (gameVersion > Vietnam193)
            {
                byte unknown_138 // hasRedArrowEffect
            }
            float walkSpeed
            float runSpeed
            float berzerkSpeed
            byte buffCount
            repeat (var i = 0; i < buffCount; i++)
            {
                uint id
                uint token
                if (buff.Record.Params.Contains(1701213281))
                {
                    byte unknown_159 // IsCreator // boolean: 0=false, nonzero=true
                }
            }
            byte flag
            if ((Flag & 2) > 0)
            {
                if (gameVersion > Thailand)
                {
                    byte count
                }
                byte options[count]
            }
            if (Flag == 6)
            {
                byte unknown_53 // maybe
                if (unknown_53 == 1)
                {
                    uint unknown_54
                }
            }
        }
    }
}
switch (obj.TypeID1): case 3:
{
    if (result.Record.IsEquip)
    {
        byte optLevel
    }
    if (!(result.Record.IsEquip))
    {
        if (result.Record.IsGold)
        {
            uint amount
        }
        if (!(result.Record.IsGold))
        {
            if (result.Record.IsQuest || result.Record.IsTrading)
            {
                ascii ownerName
            }
        }
    }
    uint uniqueId
    ushort region
    float xOffset
    float zOffset
    float yOffset
    short angle
    byte hasOwner // boolean: 0=false, nonzero=true
    if (result.HasOwner)
    {
        uint ownerJID
    }
    byte rarity
}
switch (obj.TypeID1): case 4:
{
    uint uniqueId
    ushort region
    float xOffset
    float zOffset
    float yOffset
    short angle
    byte unknown_59 // unkByte0
    byte unkByte1
    byte unknown_61 // unkByte2
    byte unkByte3
    if (unkByte3 == 1)
    {
        uint unknown_67 // unkUINT0
        uint unknown_68 // unkUINT1
    }
    if (!(unkByte3 == 1))
    {
        if (unkByte3 == 6)
        {
            ascii ownerName
            uint ownerUniqueId
        }
    }
    if (unkByte1 == 1)
    {
        uint unknown_79 // unkUInt3
        byte unknown_80 // unkByte5
    }
}
if (!false)
{
    if (obj.TypeID1 == 1 || obj.TypeID1 == 4)
    {
        byte unknown_312 // 1 = Normal, 3 = Spawning, 4 = Running
    }
    if (!(obj.TypeID1 == 1 || obj.TypeID1 == 4))
    {
        if (obj.TypeID1 == 3)
        {
            byte unknown_316 // DropSource
            uint unknown_317 // DropUID
        }
    }
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/AutoInverstType.cs
[Flags]
enum AutoInverstType : byte
{
    None = 0,
    Beginner = 1,
    Helpful = 2,
}

// Library/RSBot.Core/Objects/BodyState.cs
enum BodyState : int
{
    None = 0,
    Hwan = 1,
    Untouchable = 2,
    GameMasterInvincible = 3,
    GameMasterInvisible = 4,
    Berzerk = 5,
    Stealth = 6,
    Invisible = 7,
}

// Library/RSBot.Core/Objects/FortSiegeAuthority.cs
enum FortSiegeAuthority : int
{
    None = 0,
    Commander = 1,
    DeputyCommander = 2,
    FortressWarAdministrator = 4,
    ProductionAdministrator = 8,
    TrainingAdministrator = 16,
    MilitaryEngineer = 32,
}

// Library/RSBot.Core/Objects/InteractMode.cs
enum InteractMode : int
{
    None = 0,
    P2P = 2,
    P2N_TALK2 = 3,
    P2N_TALK = 4,
    OPNMKT_DEAL = 6,
}

// Library/RSBot.Core/Objects/JobType.cs
enum JobType : int
{
    None = 0,
    Trade = 1,
    Thief = 2,
    Hunter = 3,
}

// Library/RSBot.Core/Objects/LifeState.cs
enum LifeState : int
{
    Alive = 1,
    Dead = 2,
}

// Library/RSBot.Core/Objects/MonsterRarity.cs
enum MonsterRarity : byte
{
    General = 0,
    Champion = 1,
    Unique = 3,
    Giant = 4,
    Titan = 5,
    Elite = 6,
    EliteStrong = 7,
    Unique2 = 8,
    GeneralParty = 16,
    ChampionParty = 17,
    UniqueParty = 19,
    GiantParty = 20,
    TitanParty = 21,
    EliteParty = 22,
    Unique2Party = 24,
    Event = 255,
}

// Library/RSBot.Core/Objects/MotionState.cs
enum MotionState : int
{
    None = 0,
    Walking = 2,
    Running = 3,
    Sitting = 4,
}

// Library/RSBot.Core/Objects/MovementType.cs
enum MovementType : int
{
    Walking = 0,
    Running = 1,
}

// Library/RSBot.Core/Client/ReferenceObjects/ObjectRarity.cs
[Flags]
enum ObjectRarity : byte
{
    ClassA = 0,
    ClassB = 1,
    ClassC = 2,
    ClassD = 3,
    ClassE = 4,
    ClassF = 5,
    ClassG = 6,
    ClassH = 7,
    ClassI = 8,
}

// Library/RSBot.Core/Objects/PvpFlag.cs
enum PvpFlag : int
{
    None = 0,
    Red = 1,
    Gray = 2,
    Blue = 3,
    White = 4,
    Gold = 5,
}

// Library/RSBot.Core/Objects/PvpState.cs
enum PvpState : int
{
    None = 0,
    Purple = 1,
    Red = 2,
    Unknown = 3,
}

// Library/RSBot.Core/Objects/ScrollMode.cs
enum ScrollMode : int
{
    None = 0,
    ReturnScroll = 1,
    BanditReturnScroll = 2,
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
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3015)]
    public sealed class EntitySingleSpawnResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                NetGuard.Hot.InventoryNpcTracking.Apply(packet, client);
                var response = new PacketHandlingResult();
                return new PacketHandlingResult();
                var h = client.Host()!;
                var character = client.CurrentChar();
                try
                {
                    var uniqId1 = packet.ReadUInt32();
                    var uniqId2 = packet.ReadUInt32();
                    // Happens on spawn?
                    if (uniqId2 == character.UniqueID)
                    {
                        Custom.WriteLine($"Nigger?");
                        return new PacketHandlingResult();
                    }
        
                    Custom.WriteLine($"0x3015 uniqId1: {uniqId1} uniqId2: {uniqId2}");
                    switch (character.LastUseItem)
                    {
                        /*
                                Attack pets
                            */
                        case 0x08CD: // Item mall
                        case 0x08CC: // Normal
                        {
                        //character.HasAttackPetSummoned = true;
                        //character.AttackPetID = uniqId1;
                        }
        
                            break;
                        /*
                                Grab pets
                            */
                        case 0x10CD:
                        {
                        //character.HasGrabPetSummoned = true;
                        //character.GrabPetID = uniqId1;
                        }
        
                            break;
                    }
        
                    character.LastUseItem = 0;
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[S->C] 0x3015 {ex}", ConsoleColor.Red);
                }
        
                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3015 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}