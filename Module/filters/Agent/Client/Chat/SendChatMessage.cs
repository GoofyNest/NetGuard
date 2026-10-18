using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7025)]
    public sealed class SendChatMessage : IFilter
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