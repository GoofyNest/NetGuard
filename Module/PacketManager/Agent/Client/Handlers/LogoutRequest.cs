using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using Module;
using _Agent = Module.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.PacketManager.Agent.ClientPackets.Shard;

namespace Module.PacketManager.Agent.Client.Handlers
{
    public class LogoutRequest : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            if (client.exploitIwaFix)
                response.ResultType = PacketResultType.Block;

            byte action = packet.ReadUInt8();
            switch(action)
            {
                case 0x01: // Exit delay
                    break;

                case 0x02: // Restart delay
                    break;

                default:
                    response.ResultType = PacketResultType.Block;
                    break;
            }

            return response;
        }
    }
}
