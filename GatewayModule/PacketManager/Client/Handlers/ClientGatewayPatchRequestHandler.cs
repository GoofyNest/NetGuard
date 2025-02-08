using System;
using Framework;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;

namespace PacketManager.Server.Handlers
{
    public class ClientGatewayPatchRequestHandler : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            byte contentID = packet.ReadUInt8();
            string ModuleName = packet.ReadAscii();
            UInt32 version = packet.ReadUInt32();

            Custom.WriteLine($"contentID {contentID}");
            Custom.WriteLine($"ModuleName {ModuleName}");
            Custom.WriteLine($"version {version}");

            return response;
        }
    }
}
