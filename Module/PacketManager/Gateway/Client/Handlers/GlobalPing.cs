using System;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Client.Handlers
{
    public class GlobalPing : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            if (packet.GetBytes().Length != 0)
            {
                Custom.WriteLine($"Ignore packet CLIENT_GLOBAL_PING from {client.ip}", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
