/*
0x7025 | Agent | Client -> Server
Name: ChatRequestHook

// Variant: ChatRequestHook.ReplacePacket
byte type
byte unknown_25 // chatIndex
if (gameVersion > Vietnam)
{
    byte unknown_28 // has linking
}
if (gameVersion >= Chinese_Old)
{
    byte unknown_31
}
if (gameVersion in Thailand, Korean, Rigid, RuSro, Chinese, Japanese, Taiwan)
{
    unicode message
}
else
{
    ascii message
}

// Variant: Chat.SendChatPacket
byte type
byte unknown_26 // value: 1
if (gameVersion > Vietnam)
{
    byte unknown_29 // value: 0
}
if (gameVersion >= Chinese_Old)
{
    byte unknown_32 // value: 0
}
if (type == ChatType.Private)
{
    ascii reciever
}
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

using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7025)]
    public sealed class ChatRequestHook : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();
            var Settings = Main.Settings.Agent;
            Packet modified = new(0x7025);

            byte chatType = packet.ReadUInt8();
            modified.WriteUInt8(chatType);

            byte chatIndex = packet.ReadUInt8();
            modified.WriteUInt8(chatIndex);

            if (chatType == 2)
            {
                string reciever = packet.ReadAscii();
                modified.WriteAscii(reciever);
            }

            string message = packet.ReadAscii();

            var username = client.PlayerInfo.Username;
            var adminIndex = Settings.GameMasterConfig.GMs.FindIndex(m => m.Username == username);

            if (Settings.GameMasterConfig.Misc.DisableGMChat && adminIndex == -1)
            {
                if (chatType == 7)
                {
                    response.ResultType = PacketResultType.Block;
                    return response;
                }
                if (chatType == 3)
                    message = ";" + message;
            }
            else if (Settings.GameMasterConfig.EnablePermissionSystem)
            {
                if (chatType == 7 && adminIndex == -1)
                {
                    Custom.WriteLine($"Blocked GM notice from {username}");
                    response.ResultType = PacketResultType.Block;
                    return response;
                }

                if (adminIndex == -1)
                    return response;

                var _admin = Settings.GameMasterConfig.GMs[adminIndex];
                var _permission = _admin.Permissions;

                if (!_permission.Contains("SendNotice") && chatType == 7)
                {
                    Custom.WriteLine($"Blocked GM notice from {username}");
                    response.ResultType = PacketResultType.Block;
                    return response;
                }

                if (_admin.NoPinkChat && chatType == 3)
                    message = ";" + message;
            }

            modified.WriteAscii(message);
            response.ModifiedPackets.Add(new PacketList { Packet = modified, SendImmediately = true, SecurityType = SecurityType.RemoteSecurity });

            return response;
        }
    }
}