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
    public class Global_Identification : IPacketHandler
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
