using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using _Agent = Module.PacketManager.Agent.ServerPackets.Agent;
using _Global = Module.PacketManager.Agent.ServerPackets.Global;
using _Login = Module.PacketManager.Agent.ServerPackets.Login;

namespace Module.PacketManager.Agent.Server.Handlers
{
    public class _ExampleHandler : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            return response;
        }
    }
}
