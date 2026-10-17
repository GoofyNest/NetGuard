using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering
{
    /// <summary>
    /// A dynamically-compiled packet filter loaded from filters/**/*.cs.
    /// The folder the file lives in determines which dispatcher sees it:
    ///   filters/Agent/Client   -> Agent client packets (C -> S)
    ///   filters/Agent/Server   -> Agent server packets (S -> C)
    ///   filters/Gateway/Client -> Gateway client packets (C -> S)
    ///   filters/Gateway/Server -> Gateway server packets (S -> C)
    /// </summary>
    public interface IFilter
    {
        PacketHandlingResult Handle(Packet packet, SessionData client);
    }

    /// <summary>
    /// A plugin that reacts to bus messages. Instantiated once per reload,
    /// after compilation. Register handlers in the constructor.
    /// </summary>
    public interface IBusPlugin
    {
        void Register(BusDispatcher dispatcher);
    }
}