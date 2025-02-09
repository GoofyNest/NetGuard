using System;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.Services;
using SilkroadSecurityAPI;
using Module;
using static GatewayModule.Framework.Opcodes;
using static GatewayModule.Framework.Opcodes.Server;

namespace GatewayModule.PacketManager.Client.Handlers
{
    public class GlobalPing : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            if (packet.GetBytes().Length != 0)
            {
                Custom.WriteLine($"Ignore packet CLIENT_GLOBAL_PING from {client.ip}", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
