using System;
using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;
using _Agent = Module.Helpers.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.Helpers.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.Helpers.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.Helpers.PacketManager.Agent.ClientPackets.Shard;
using static Module.Helpers.PacketManager.Gateway.Opcodes;
namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class Global_Identification : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            if (packet.GetBytes().Length != 12)
            {
                Custom.WriteLine($"Ignore packet GLOBAL_IDENTIFICATION from {client.playerInfo.ipInfo.ip}", ConsoleColor.Yellow);
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
