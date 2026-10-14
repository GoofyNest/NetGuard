using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Gateway.Client.Packets
{
    public class Default : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            Custom.WriteLine($"[S->C] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Magenta);

            PacketHandlingResult response = new()
            {
                ResultType = PacketResultType.Block
            };

            return response;
        }
    }
}