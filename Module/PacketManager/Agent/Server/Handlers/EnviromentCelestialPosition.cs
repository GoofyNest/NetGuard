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
    public class EnviromentCelestialPosition : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            UInt32 uniqueID = packet.ReadUInt32();
            ushort moonPhase = packet.ReadUInt16();
            byte hour = packet.ReadUInt8();
            byte minute = packet.ReadUInt8();

            client.exploitIwaFix = false;

            return response;
        }
    }
}
