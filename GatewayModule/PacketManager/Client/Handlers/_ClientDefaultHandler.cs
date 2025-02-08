using System;
using Framework;
using Module;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;
using static Framework.Opcodes;
using static Framework.Opcodes.Server;

namespace PacketManager.Server.Handlers
{
    public class _ClientDefaultHandler : IPacketHandler
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
