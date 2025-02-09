using System;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using SilkroadSecurityAPI;
using Module;
using static GatewayModule.Framework.Opcodes;
using static GatewayModule.Framework.Opcodes.Server;

namespace GatewayModule.PacketManager.Server.Handlers
{
    public class PatchResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            client.sent_id = 1;

            return response;
        }
    }
}
