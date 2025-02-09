using AgentModule.Engine.Classes;
using SilkroadSecurityAPI;

namespace AgentModule.Framework
{
    public interface IPacketHandler
    {
        // Returns true if the packet was processed, false if it should be blocked.
        PacketHandlingResult Handle(Packet packet, SessionData client);
    }
}
