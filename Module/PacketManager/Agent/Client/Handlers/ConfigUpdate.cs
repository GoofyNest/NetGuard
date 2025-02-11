using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;
using Module.Services;

namespace Module.PacketManager.Agent.Client.Handlers
{
    public class ConfigUpdate : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            byte action = packet.ReadUInt8();

            if(action == 2) // Auto Potion
            {
                // Something with HP
                byte hp_unk = packet.ReadUInt8(); 
                byte hp_unk2 = packet.ReadUInt8();

                // Something with MP
                byte mp_unk = packet.ReadUInt8();
                byte mp_unk2 = packet.ReadUInt8();

                // Unknown right now
                byte unk5 = packet.ReadUInt8();
                byte unk6 = packet.ReadUInt8();

                byte delay = packet.ReadUInt8();

            }

            return response;
        }
    }
}
