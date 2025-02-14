using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class LogoutRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            if (client.shardSettings.exploitIwaFix)
                response.ResultType = PacketResultType.Block;

            byte action = packet.ReadUInt8();

            if(action > 2)
                response.ResultType = PacketResultType.Block;

            switch (action)
            {
                case 0x01: // Exit delay
                    break;

                case 0x02: // Restart delay
                    break;

                default:
                    response.ResultType = PacketResultType.Block;
                    break;
            }

            return response;
        }
    }
}
