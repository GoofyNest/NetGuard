using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

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

            client.agentSettings.sentJoinRequest = true;

            return response;
        }
    }
}
