using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Server.Handlers
{
    public class LoginIBUVChallange : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            var modifiedPacket = new Packet(GATEWAY_LOGIN_IBUV_ANSWER, false);
            modifiedPacket.WriteAscii("b");

            response.securityType = SecurityType.RemoteSecurity;
            response.SendImmediately = true;
            response.ModifiedPacket = modifiedPacket;



            return response;
        }
    }
}
