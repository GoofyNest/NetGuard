using System;
using AgentModule.Engine.Classes;
using AgentModule.Framework;
using SilkroadSecurityAPI;
using Module;
using static AgentModule.Framework.Opcodes;
using static AgentModule.Framework.Opcodes.Server;

namespace AgentModule.PacketManager.Client.Handlers
{
    public class LogoutRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            if (client.exploitIwaFix)
                response.ResultType = PacketResultType.Block;

            byte action = packet.ReadUInt8();
            switch(action)
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
