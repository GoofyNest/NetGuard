using Module.Networking;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server.Packets
{
    public class CharData : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            /*
                The CharData packet I am using is from DuckSoup's project
                https://github.com/ducksoup-sro/ducksoup

                I fixed the CharData parsing for JSRO files since most of the stuff is missing in older files.

                <3
            */
            PacketHandlingResult response = new();

            if (client.Agent.CharData == null)
                client.Agent.CharData = new(0x3013);

            var Settings = Main.Settings.Agent;

            var adminIndex = Settings.GameMasterConfig.GMs.FindIndex(m => m.Username == client.PlayerInfo.AccInfo.Username);

            int count1 = 0, count2 = 0;

            List<byte> packet_test = new();

            for (var i = 0; i < packet.GetBytes().Length; i++)
            {
                var result = packet.ReadUInt8();

                if (result == 255)
                    count1++;

                packet_test.Add(result);
            }

            for (var i = 0; i < packet_test.Count; i++)
            {
                var result = packet_test[i];

                client.Agent.CharData.WriteUInt8(result);

                if (Settings.GameMasterConfig.Misc.DisableGMConsole)
                {
                    if (result == 255)
                    {
                        count2++;

                        // Ensure only the last 255 byte
                        if(count1 == count2 && adminIndex == -1)
                        {
                            // Write the next 8 bytes after 255
                            for (int j = 1; j < 9; j++)
                            {
                                client.Agent.CharData.WriteUInt8(packet_test[i + j]);
                            }

                            // disable gm console
                            client.Agent.CharData.WriteUInt8(0);

                            // Skip 9 bytes ahead
                            i += 9;

                            continue;
                        }
                    }
                }
            }

            response.ModifiedPackets.Add(new PacketList() { Packet = client.Agent.CharData });


            return response;
        }
    }
}
