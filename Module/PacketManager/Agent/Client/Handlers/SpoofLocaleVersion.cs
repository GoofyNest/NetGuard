using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;

namespace Module.PacketManager.Agent.Client.Handlers
{ 
    public class SpoofLocaleVersion : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            var modifiedPacket = new Packet(AGENT_GAME_READY);
            response.ModifiedPacket = modifiedPacket;

            response.SendImmediately = false;

            return response;
        }
    }
}
