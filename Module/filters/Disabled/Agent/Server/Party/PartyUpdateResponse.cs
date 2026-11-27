/*
0x3864 | Agent | Server -> Client
Name: PartyUpdateResponse

// Variant: PartyUpdateResponse.Invoke
byte type
switch (type): case PartyUpdateType.Joined:
{
    byte unknown_98 // FF
    uint memberId
    ascii resultName
    uint objectId
    byte level
    byte healthMana // 0-A|0-A -> 0%-100%|0%-100%
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
    short worldId
    short layerId
    ascii guild
    byte unknown_107 // 04
    uint masteryId1
    uint masteryId2
}
switch (type): case PartyUpdateType.Leave:
{
    uint memberLeft
    byte unknown_57
}
switch (type): case PartyUpdateType.Member:
{
    uint memberId
    byte memberUpdateType
    switch (memberUpdateType): case PartyMemberUpdateType.NameRefObjID:
    {
        ascii memberName
        uint objectId
    }
    switch (memberUpdateType): case PartyMemberUpdateType.HPMP:
    {
        byte healthMana // 0-A|0-A -> 0%-100%|0%-100%
    }
    switch (memberUpdateType): case PartyMemberUpdateType.Mastery:
    {
        uint masteryId1
        uint masteryId2
    }
    switch (memberUpdateType): case PartyMemberUpdateType.Level:
    {
        byte level
    }
    switch (memberUpdateType): case PartyMemberUpdateType.Position:
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
        short worldId
        short layerId
    }
    switch (memberUpdateType): case PartyMemberUpdateType.Guild:
    {
        ascii guild
    }
}
switch (type): case PartyUpdateType.Leader:
{
    uint leader
}
switch (type): case PartyUpdateType.LeaderChange:
{
    uint leader
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/Party/PartyMemberUpdateType.cs
enum PartyMemberUpdateType : int
{
    NameRefObjID = 1,
    Level = 2,
    HPMP = 4,
    Mastery = 8,
    Position = 32,
    Guild = 64,
}

// Library/RSBot.Core/Objects/Party/PartyUpdateType.cs
enum PartyUpdateType : int
{
    Dismissed = 1,
    Joined = 2,
    Leave = 3,
    Member = 6,
    Leader = 7,
    LeaderChange = 9,
}
*/

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3864)]
    public sealed class PartyUpdateResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3864 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
