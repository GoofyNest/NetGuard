using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;

namespace Module.PacketManager.Agent.Client.Handlers
{
    public class DefaultBlock : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            response.ResultType = PacketResultType.Block;

            return response;
        }
    }
}
