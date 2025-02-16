using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Server.Packets
{
    public class CharDataLoadingStart : IPacketHandler
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

            client.Agent.CharData = null!;

            return response;
        }
    }
}
