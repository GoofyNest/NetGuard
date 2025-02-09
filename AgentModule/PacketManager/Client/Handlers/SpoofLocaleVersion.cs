using System;
using AgentModule.Engine.Classes;
using AgentModule.Framework;
using SilkroadSecurityAPI;
using Module;
using static AgentModule.Framework.Opcodes;
using static AgentModule.Framework.Opcodes.Server;

namespace AgentModule.PacketManager.Client.Handlers
{
    public class SpoofLocaleVersion : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            var modifiedPacket = new Packet(AGENT_GAME_READY);
            response.ModifiedPacket = modifiedPacket;

            response.SendImmediately = false;

            return response;
        }
    }
}
