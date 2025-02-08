using System;
using Framework;
using Module;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;
using static Framework.Opcodes;
using static Framework.Opcodes.Server;

namespace PacketManager.Server.Handlers
{
    public class ClientGlobalPing : IPacketHandler
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
