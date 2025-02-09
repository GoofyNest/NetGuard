using System;
using AgentModule.Engine.Classes;
using AgentModule.Framework;
using SilkroadSecurityAPI;
using Module;
using static AgentModule.Framework.Opcodes;
using static AgentModule.Framework.Opcodes.Server;

namespace AgentModule.PacketManager.Client.Handlers
{
    public class DefaultBlock : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            response.ResultType = PacketResultType.Block;

            return response;
        }
    }
}
