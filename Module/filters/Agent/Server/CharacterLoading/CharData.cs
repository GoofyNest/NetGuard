using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3013)]
    public sealed class CharData : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            if (h.Agent.CharData == null)
                h.Agent.CharData = new Packet(0x3013);

            var Settings = Main.Settings.Agent;
            var adminIndex = Settings.GameMasterConfig.GMs.FindIndex(m => m.Username == client.PlayerInfo.Username);

            int count1 = 0, count2 = 0;
            List<byte> packet_test = new();

            for (var i = 0; i < packet.GetBytes().Length; i++)
            {
                var result = packet.ReadUInt8();
                if (result == 255) count1++;
                packet_test.Add(result);
            }

            for (var i = 0; i < packet_test.Count; i++)
            {
                var result = packet_test[i];
                h.Agent.CharData.WriteUInt8(result);

                if (Settings.GameMasterConfig.Misc.DisableGMConsole)
                {
                    if (result == 255)
                    {
                        count2++;

                        if (count1 == count2 && adminIndex == -1)
                        {
                            for (int j = 1; j < 9; j++)
                                h.Agent.CharData.WriteUInt8(packet_test[i + j]);

                            h.Agent.CharData.WriteUInt8(0);
                            i += 9;
                            continue;
                        }
                    }
                }
            }

            response.ModifiedPackets.Add(new PacketList() { Packet = h.Agent.CharData });
            return response;
        }
    }
}