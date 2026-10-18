using Module.Networking;
using Module.PacketHandler.Filtering;   // for SessionContext
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering
{
    public interface IFilter
    {
        PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx);
    }

    public interface IBusPlugin
    {
        void Register(BusDispatcher dispatcher);
    }
}