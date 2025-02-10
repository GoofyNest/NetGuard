using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;

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
