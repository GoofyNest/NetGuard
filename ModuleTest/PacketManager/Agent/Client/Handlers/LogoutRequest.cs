using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPICore;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;

namespace Module.PacketManager.Agent.Client.Handlers
{
    public class LogoutRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

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
