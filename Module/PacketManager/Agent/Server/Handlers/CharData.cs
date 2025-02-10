using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;

namespace Module.PacketManager.Agent.Server.Handlers
{
    public class CharData : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            if (client.charData == null)
                client.charData = new Packet(0x0000);

            for (var d = 0; d < packet.GetBytes().Length; d++)
                client.charData.WriteUInt8(packet.ReadUInt8());

            return response;
        }
    }
}
