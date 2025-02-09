using System;
using GatewayModule.Engine.Classes;
using GatewayModule.Framework;
using GatewayModule.Services;
using GatewayModule.SilkroadSecurityAPI;
using Module;
using static GatewayModule.Framework.Opcodes;
using static GatewayModule.Framework.Opcodes.Server;

namespace GatewayModule.PacketManager.Client.Handlers
{
    public class Default : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new PacketHandlingResult();

            response.ModifiedPacket = null;

            Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Yellow);
            response.ResultType = PacketResultType.Block;

            return response;
        }
    }
}
