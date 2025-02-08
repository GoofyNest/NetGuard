using System;
using Framework;
using Module;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;
using static Framework.Opcodes.Server;

namespace PacketManager.Server.Handlers
{
    public class ServerHandshakeHandler : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            response.SendImmediately = true;
            response.ResultType = PacketResultType.SkipSending;

            return response;
        }
    }
}
