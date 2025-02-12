using System;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;
using Module;
using _Agent = Module.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.PacketManager.Agent.ClientPackets.Shard;
namespace Module.PacketManager.Agent.Client.Handlers
{
    public class Global_Identification : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

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
