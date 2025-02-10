using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPICore;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Server.Handlers
{
    public class ShardListResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            client.sent_list = 1;

            return response;
        }
    }
}
