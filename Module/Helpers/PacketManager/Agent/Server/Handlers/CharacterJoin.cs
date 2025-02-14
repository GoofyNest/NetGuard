using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using _Agent = Module.Helpers.PacketManager.Agent.ServerPackets.Agent;
using _Global = Module.Helpers.PacketManager.Agent.ServerPackets.Global;
using _Login = Module.Helpers.PacketManager.Agent.ServerPackets.Login;

namespace Module.Helpers.PacketManager.Agent.Server.Handlers
{
    public class CharacterJoin : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null!;

            if (client.playerInfo.charInfo == null)
                client.playerInfo.charInfo = new();

            var charInfo = client.playerInfo.charInfo;

            var index = charInfo.FindIndex(m => m.charname == client.playerInfo.currentChar);

            if (index == -1)
                charInfo.Add(new CharacterInformation() { charname = client.playerInfo.currentChar });

            client.playerInfo.sentJoinRequest = true;

            return response;
        }
    }
}
