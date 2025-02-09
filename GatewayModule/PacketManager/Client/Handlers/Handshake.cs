using System;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.SilkroadSecurityAPI;
using Module;
using static GatewayModule.Framework.Opcodes.Server;

namespace GatewayModule.PacketManager.Client.Handlers
{
    public class Handshake : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            response.SendImmediately = false;
            response.ResultType = PacketResultType.SkipSending;

            return response;
        }
    }
}
