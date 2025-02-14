using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using _Agent = Module.Helpers.PacketManager.Agent.ClientPackets.Agent;
using _Global = Module.Helpers.PacketManager.Agent.ClientPackets.Global;
using _Login = Module.Helpers.PacketManager.Agent.ClientPackets.Login;
using _Shard = Module.Helpers.PacketManager.Agent.ClientPackets.Shard;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class Handshake : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            response.SendImmediately = false;
            response.ResultType = PacketResultType.SkipSending;

            return response;
        }
    }
}
