using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class ChatMessage : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var Settings = Main.Settings;

            Packet modified = new(0x7025);

            /*
                1 = /TEXTCHAT
                2 = Private Message
                3 = General Chat
                4 = Party chat
                6 = Global Chat
                7 = GM Notice
            */
            int chatType = packet.ReadUInt8();
            modified.WriteUInt8(chatType);

            int chatIndex = packet.ReadUInt8();
            modified.WriteUInt8(chatIndex);

            if(chatType == 2)
            {
                string reciever = packet.ReadAscii();
                modified.WriteAscii(reciever);
            }

            string message = packet.ReadAscii();

            var adminIndex = Settings.Agent.GameMasters.FindIndex(m => m.Username == client.PlayerInfo.AccInfo.Username);
            if (adminIndex == -1 && chatType == 7)
            {
                Custom.WriteLine($"Blocked GM notice from {client.PlayerInfo.AccInfo.Username}");
                response.ResultType = PacketResultType.Block;
                return response;
            }

            if (adminIndex == -1)
                return response;

            var _admin = Settings.Agent.GameMasters[adminIndex];
            var _permission = _admin.Permissions;

            if(!_permission.Contains("SendNotice") && chatType == 7)
            {
                Custom.WriteLine($"Blocked GM notice from {client.PlayerInfo.AccInfo.Username}");
                response.ResultType = PacketResultType.Block;
                return response;
            }

            if (_admin.NoPinkChat && chatType == 3)
                message = ";" + message;

            modified.WriteAscii(message);

            response.ModifiedPackets.Add(new PacketList() { Packet = modified, SendImmediately = true, SecurityType = SecurityType.RemoteSecurity });

            return response;
        }
    }
}
