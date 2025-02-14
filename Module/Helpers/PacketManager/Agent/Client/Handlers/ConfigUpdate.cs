using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using _Agent = Module.Helpers.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.Helpers.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.Helpers.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.Helpers.PacketManager.Agent.ClientPackets.Shard;
using Module.Services;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class ConfigUpdate : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            byte action = packet.ReadUInt8();

            if (action == 2) // Auto Potion
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
