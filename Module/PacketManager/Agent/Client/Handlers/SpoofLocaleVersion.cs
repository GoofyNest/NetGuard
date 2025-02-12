using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using _Agent = Module.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.PacketManager.Agent.ClientPackets.Shard;

namespace Module.PacketManager.Agent.Client.Handlers
{ 
    public class SpoofLocaleVersion : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            var modifiedPacket = new Packet((ushort)_Agent.GameReady);
            response.ModifiedPacket = modifiedPacket;

            response.SendImmediately = false;

            return response;
        }
    }
}
