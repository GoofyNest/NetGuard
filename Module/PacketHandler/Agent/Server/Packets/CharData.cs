using Module.Networking;
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
                client.Agent.CharData = new(0x0000);

            for (var d = 0; d < packet.GetBytes().Length; d++)
                client.Agent.CharData.WriteUInt8(packet.ReadUInt8());

            return response;
        }
    }
}
