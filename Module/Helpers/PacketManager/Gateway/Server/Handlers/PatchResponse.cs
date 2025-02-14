using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Client;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Server;

namespace Module.Helpers.PacketManager.Gateway.Server.Handlers
{
    public class PatchResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            client.gatewaySettings.sentPatchResponse = true;

            return response;
        }
    }
}
