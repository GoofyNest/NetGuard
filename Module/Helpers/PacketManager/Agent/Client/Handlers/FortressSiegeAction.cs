using System.Text.RegularExpressions;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;

namespace Module.Helpers.PacketManager.Agent.Client.Handlers
{
    public class FortressSiegeAction : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            var message = packet.ReadAscii();

            if (Regex.IsMatch(message, @"['""\-]"))
            {
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}
