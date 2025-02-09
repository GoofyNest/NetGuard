using System;
using AgentModule.Engine.Classes;
using AgentModule.Framework;
using SilkroadSecurityAPI;
using Module;
using static AgentModule.Framework.Opcodes;
using static AgentModule.Framework.Opcodes.Server;
using AgentModule.Services;

namespace AgentModule.PacketManager.Server.Handlers
{
    public class AuthResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            byte errorCode = packet.ReadUInt8();

            if (errorCode == 0x01)
                client.inCharSelection = true;

            Custom.WriteLine($"SERVER_AGENT_AUTH_RESPONSE {errorCode}", ConsoleColor.Cyan);

            return response;
        }
    }
}
