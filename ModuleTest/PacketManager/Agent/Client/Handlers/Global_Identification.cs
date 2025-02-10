using System;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPICore;
using Module;
using static Module.PacketManager.Agent.Opcodes.Client;
using static Module.PacketManager.Agent.Opcodes.Server;

namespace Module.PacketManager.Agent.Client.Handlers
{
    public class Global_Identification : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

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
