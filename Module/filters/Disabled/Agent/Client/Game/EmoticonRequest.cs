/*
0x3091 | Agent | Client -> Server
Name: EmoticonRequest

// Variant: EmoticonRequest.Invoke
if (!PluginConfig.Enabled)
{
    // Stop parsing this variant.
}
byte type

// Enum values used by this packet and its nested structures.
// RSBot source definitions (AGPL-3.0); documentation only.

// Plugins/RSBot.CommandCenter/Components/Emoticons.cs
enum EmoticonType : byte
{
    Hi = 0,
    Greeting = 1,
    Rush = 2,
    Joy = 3,
    No = 4,
    Yes = 5,
    Smile = 6,
}
*/

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x3091)]
    public sealed class EmoticonRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            return new PacketHandlingResult();
        }
    }
}
