using System;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPICore;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Client.Handlers
{
    public class PatchRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            byte contentID = packet.ReadUInt8();
            string ModuleName = packet.ReadAscii();
            uint version = packet.ReadUInt32();

            Custom.WriteLine($"contentID {contentID}");
            Custom.WriteLine($"ModuleName {ModuleName}");
            Custom.WriteLine($"version {version}");

            return response;
        }
    }
}
