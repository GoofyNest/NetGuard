using System;
using AgentModule.Engine.Classes;
using AgentModule.Framework;
using SilkroadSecurityAPI;
using Module;
using static AgentModule.Framework.Opcodes;
using static AgentModule.Framework.Opcodes.Server;

namespace AgentModule.PacketManager.Server.Handlers
{
    public class CharacterSelectionResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            client.inCharSelection = true;

            return response;
        }
    }
}
