/*
0xB007 | Agent | Server -> Client
Name: CharacterListing

// Variant: CharacterListing.Invoke
byte type
byte result
if (type != 2)
{
    // Stop parsing this variant.
}
if (result != 1)
{
    // Stop parsing this variant.
}
byte charCount
repeat (var i = 0; i < charCount; i++)
{
    int unknown_51 // Model
    ascii name
    if (gameVersion >= Chinese)
    {
        ascii unknown_56
    }
    byte unknown_58 // Scale
    byte level
    ulong unknown_60 // EXP
    ushort unknown_61 // Strength
    ushort unknown_62 // Intelligence
    ushort unknown_63 // Stat point(s)
    if (gameVersion >= Chinese_Old)
    {
        int unknown_66 // skill point
    }
    int unknown_68 // Health
    int unknown_69 // Mana
    if (gameVersion >= Chinese_Old)
    {
        ushort unknown_72 // Region
    }
    byte characterDeletionFlag // boolean: 0=false, nonzero=true
    if (characterDeletionFlag)
    {
        int unknown_77 // Time till deletion
    }
    if (gameVersion >= Chinese)
    {
        uint unknown_80 // last logged out timestamp
    }
    byte unknown_82 // Has guild?
    byte grantNameFlag
    if (grantNameFlag == 0x01)
    {
        ascii unknown_85
    }
    byte unknown_87 // Academy
    byte itemCount
    repeat (var iItem = 0; iItem < itemCount; iItem++)
    {
        int unknown_93 // Item identifier
        byte unknown_94 // Item plus value (enhancement)
    }
    if (gameVersion >= Thailand)
    {
        byte avatarCount
        repeat (var iAvatar = 0; iAvatar < avatarCount; iAvatar++)
        {
            int unknown_103 // Avatar identifier
            byte unknown_104 // Avatar plus value (enhancement)
        }
    }
}
if (selectedAccount == null)
{
    // Stop parsing this variant.
}
if (!(!string.IsNullOrWhiteSpace(ProfileManager.SelectedCharacter) && lobbyCharacters.Any(p => p.name == ProfileManager.SelectedCharacter)))
{
    if (string.IsNullOrWhiteSpace(selectedAccount.SelectedCharacter) || !lobbyCharacters.Any(p => p.name == selectedAccount.SelectedCharacter))
    {
        if (charCount == 0)
        {
            // Stop parsing this variant.
        }
        if (!(GlobalConfig.Get<bool>("RSBot.General.CharacterAutoSelectFirst")))
        {
            if (!(GlobalConfig.Get<bool>("RSBot.General.CharacterAutoSelectHigher")))
            {
                // Stop parsing this variant.
            }
        }
    }
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
    [Filter(0xB007)]
    public sealed class CharacterListing : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                client.Agent.InCharSelectionScreen = true;
                return new PacketHandlingResult();
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0xB007 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}