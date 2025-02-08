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
    public class ClientGlobalIdentification : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            if (packet.GetBytes().Length != 12)
            {
                Custom.WriteLine($"Ignore packet GLOBAL_IDENTIFICATION from {client.ip}", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }
            else
            {
                Custom.WriteLine($"Should recieve?");
                response.ResultType = PacketResultType.DoReceive;
            }



            return response;
        }
    }
}
