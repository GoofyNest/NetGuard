using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using _Agent = Module.Helpers.PacketManager.Agent.ServerPackets.Agent;
using _Global = Module.Helpers.PacketManager.Agent.ServerPackets.Global;
using _Login = Module.Helpers.PacketManager.Agent.ServerPackets.Login;
using Module.Services;

namespace Module.Helpers.PacketManager.Agent.Server.Handlers
{
    public class AuthResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            byte errorCode = packet.ReadUInt8();

            if (errorCode == 0x01)
                client.shardSettings.inCharSelection = true;

            Custom.WriteLine($"SERVER_AGENT_AUTH_RESPONSE {errorCode}", ConsoleColor.Cyan);

            return response;
        }
    }
}
