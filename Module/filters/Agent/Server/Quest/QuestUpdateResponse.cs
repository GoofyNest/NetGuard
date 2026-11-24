/*
0x30D5 | Agent | Server -> Client
Name: QuestUpdateResponse

// Variant: QuestUpdateResponse.Invoke
byte type
uint questId
if (quest == null)
{
    // Stop parsing this variant.
}
if (type == QuestUpdateType.Abandon)
{
    if (!Game.Player.QuestLog.ActiveQuests.TryGetValue(questId, out var playerQuest))
    {
        // Stop parsing this variant.
    }
}
if (type == QuestUpdateType.Add)
{
    if (gameVersion == Vietnam274)
    {
        ushort achievementAmount
        ushort requiredShareParty
    }
    if (!(gameVersion == Vietnam274))
    {
        byte achievementAmount
        byte requiredShareParty
    }
    if (gameVersion >= Chinese)
    {
        byte unknown1
        byte unknown2
    }
    byte type
    if ((activeQuest.Type & QuestType.Time) == QuestType.Time)
    {
        int remainingTime
    }
    if ((activeQuest.Type & QuestType.Status) == QuestType.Status)
    {
        byte status
    }
    if ((activeQuest.Type & QuestType.Objective) == QuestType.Objective)
    {
        byte objectiveCount
        repeat (var i = 0; i < objectiveCount; i++)
        {
            byte id
            byte inProgress // boolean: 0=false, nonzero=true
            ascii nameStrId
            byte tasksCount
            uint tasks[tasksCount]
        }
    }
    if ((activeQuest.Type & QuestType.RefObjects) == QuestType.RefObjects)
    {
        byte npcsCount
        uint npcs[npcsCount]
    }
}
if (type == QuestUpdateType.Update)
{
    if (gameVersion == Vietnam274)
    {
        ushort achievementAmount
        ushort requiredShareParty
    }
    if (!(gameVersion == Vietnam274))
    {
        byte achievementAmount
        byte requiredShareParty
    }
    if (gameVersion >= Chinese)
    {
        byte unknown1
        byte unknown2
    }
    byte type
    if ((activeQuest.Type & QuestType.Time) == QuestType.Time)
    {
        int remainingTime
    }
    if ((activeQuest.Type & QuestType.Status) == QuestType.Status)
    {
        byte status
    }
    if ((activeQuest.Type & QuestType.Objective) == QuestType.Objective)
    {
        byte objectiveCount
        repeat (var i = 0; i < objectiveCount; i++)
        {
            byte id
            byte inProgress // boolean: 0=false, nonzero=true
            ascii nameStrId
            byte tasksCount
            uint tasks[tasksCount]
        }
    }
    if ((activeQuest.Type & QuestType.RefObjects) == QuestType.RefObjects)
    {
        byte npcsCount
        uint npcs[npcsCount]
    }
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/Quests/QuestStatus.cs
enum QuestStatus : byte
{
    Initialized = 1,
    CompletedButNotSupplied = 2,
    Completed = 3,
    CompletedXTimes = 4,
    Unavailable = 5,
    Cancelled = 6,
    StartedByUser = 7,
    CompletedByUserButNotSupplied = 8,
}

// Library/RSBot.Core/Objects/Quests/QuestType.cs
[Flags]
enum QuestType : byte
{
    UnkBit0 = 1,
    UnkBit1 = 2,
    Time = 4,
    Status = 8,
    Objective = 16,
    UnkBit5 = 32,
    RefObjects = 64,
    UnkBit7 = 128,
}

// Library/RSBot.Core/Objects/Quests/QuestUpdateType.cs
enum QuestUpdateType : byte
{
    Add = 1,
    Update = 2,
    Remove = 3,
    Abandon = 4,
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

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x30D5)]
    public sealed class QuestUpdateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x30D5 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
