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

            if(message.Contains("gg"))
            {
                var sequence = new List<(string Name, Packet Packet)>();

                void AddAction(byte[] data)
                {
                    if (data == null || data.Length == 0) return;

                    var p = new Packet(0x7074, false);
                    foreach (var b in data) p.WriteUInt8(b);
                    sequence.Add(("Action 0x7074", p));
                }

                /*
                    [C -> S][7074]
                    01                                                ................
                    05                                                ................
                    29 0C 00 00                                       )...............
                    00                                                ................

                */

                // Add your payload(s) here
                AddAction(new byte[] { 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01 });
                AddAction(new byte[] { 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01 });
                AddAction(new byte[] { 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01 });
                AddAction(new byte[] { 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01 });
                AddAction(new byte[] { 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01 });
                AddAction(new byte[] { 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01 });

                // Use sequence.Count instead of hardcoding the number
                int count = sequence.Count;

                for (int i = 0; i < count; i++)
                {
                    var (name, p) = sequence[i];
                    Custom.WriteLine($"  [{i + 1}/{count}] Sending: {name}");
                    
                    response.ModifiedPackets.Add(new PacketList()
                    {
                        Packet = p,
                        SendImmediately = true,
                        SecurityType = SecurityType.RemoteSecurity
                    });
                }
            }

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