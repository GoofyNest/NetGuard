using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPICore;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;

namespace Module.PacketManager.Agent.Server.Handlers
{
    public class Handshake : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            response.SendImmediately = true;
            response.ResultType = PacketResultType.SkipSending;

            return response;
        }
    }
}
