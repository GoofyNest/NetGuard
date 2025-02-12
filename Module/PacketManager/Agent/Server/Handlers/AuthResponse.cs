using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using _Agent = Module.PacketManager.Agent.ServerPackets.Agent;
using _Global = Module.PacketManager.Agent.ServerPackets.Global;
using _Login = Module.PacketManager.Agent.ServerPackets.Login;
using Module.Services;

namespace Module.PacketManager.Agent.Server.Handlers
{
    public class AuthResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            byte errorCode = packet.ReadUInt8();

            if (errorCode == 0x01)
                client.inCharSelection = true;

            Custom.WriteLine($"SERVER_AGENT_AUTH_RESPONSE {errorCode}", ConsoleColor.Cyan);

            return response;
        }
    }
}
