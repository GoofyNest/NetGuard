using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using _Agent = Module.Helpers.PacketManager.Agent.ServerPackets.Agent;
using _Global = Module.Helpers.PacketManager.Agent.ServerPackets.Global;
using _Login = Module.Helpers.PacketManager.Agent.ServerPackets.Login;

namespace Module.Helpers.PacketManager.Agent.Server.Handlers
{
    public class EnviromentCelestialPosition : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            uint uniqueID = packet.ReadUInt32();
            ushort moonPhase = packet.ReadUInt16();
            byte hour = packet.ReadUInt8();
            byte minute = packet.ReadUInt8();

            client.shardSettings.exploitIwaFix = false;

            return response;
        }
    }
}
