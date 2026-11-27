/*
0x3026 | Agent | Server -> Client
Name: ChatResponse

// Variant: ChatResponse.Invoke
byte type
switch (type): case ChatType.All: case ChatType.AllGM:
{
    uint senderId
    if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
    {
        unicode message
    }
    else
    {
        ascii message
    }
    if (senderId != Game.Player.UniqueId)
    {
        if (!SpawnManager.TryGetEntity<SpawnedPlayer>(senderId, out var player))
        {
            // Stop parsing this variant.
        }
    }
}
switch (type): case ChatType.Notice:
{
    if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
    {
        unicode message
    }
    else
    {
        ascii message
    }
}
switch (type): default:
{
    ascii sender
    if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
    {
        unicode message
    }
    else
    {
        ascii message
    }
}

// Variant: ChatResponse.Invoke
byte type
switch (type): case ChatType.All: case ChatType.AllGM:
{
    uint senderId
    if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
    {
        unicode message
    }
    else
    {
        ascii message
    }
    if (senderId == Game.Player.UniqueId)
    {
        // Stop parsing this variant.
    }
    if (!SpawnManager.TryGetEntity(senderId, out player))
    {
        // Stop parsing this variant.
    }
}
switch (type): case ChatType.Private: case ChatType.Party:
{
    ascii sender
    if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
    {
        unicode message
    }
    else
    {
        ascii message
    }
    if (!SpawnManager.TryGetEntity(p => p.Name == sender, out player))
    {
        // Stop parsing this variant.
    }
}

// Variant: Game.ShowNotification
byte notice
if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
{
    unicode message
}
else
{
    ascii message
}

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Library/RSBot.Core/Objects/ChatType.cs
enum ChatType : int
{
    All = 1,
    Private = 2,
    AllGM = 3,
    Party = 4,
    Guild = 5,
    Global = 6,
    Notice = 7,
    Stall = 9,
    Union = 11,
    Npc = 13,
    Academy = 16,
}

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

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3026)]
    public sealed class ChatResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x3026 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}
