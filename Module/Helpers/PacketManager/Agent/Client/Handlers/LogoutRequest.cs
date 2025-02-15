using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class LogoutRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            if (client.Agent.InCharSelectionScreen)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, attempt to crash people", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            byte action = packet.ReadUInt8();

            if(action > 2)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, attempt to crash people", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

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
