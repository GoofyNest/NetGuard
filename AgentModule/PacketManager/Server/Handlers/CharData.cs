using System;
using AgentModule.Engine.Classes;
using AgentModule.Framework;
using SilkroadSecurityAPI;
using Module;
using static AgentModule.Framework.Opcodes;
using static AgentModule.Framework.Opcodes.Server;

namespace AgentModule.PacketManager.Server.Handlers
{
    public class CharData : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            if (client.charData == null)
                client.charData = new Packet(0x0000);

            for (var d = 0; d < packet.GetBytes().Length; d++)
                client.charData.WriteUInt8(packet.ReadUInt8());

            return response;
        }
    }
}
