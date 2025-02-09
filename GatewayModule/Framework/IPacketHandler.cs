using GatewayModule.Engine.Classes;
using SilkroadSecurityAPI;

namespace GatewayModule.Framework
{
    public interface IPacketHandler
    {
        // Returns true if the packet was processed, false if it should be blocked.
        PacketHandlingResult Handle(Packet packet, SessionData client);
    }
}
