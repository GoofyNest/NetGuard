using System.Collections.Concurrent;
using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering
{
    /// <summary>
    /// Handed to Landings and Filters. Gives them a safe way to send packets
    /// to either peer without touching the security objects directly.
    /// All sends are queued and flushed by the owning Module after the
    /// current packet finishes — safe to call from a sync Handle().
    /// </summary>
    public sealed class SessionContext
    {
        private readonly SessionData _session;
        private readonly ISessionTransport _transport;

        internal SessionContext(SessionData session, ISessionTransport transport)
        {
            _session = session;
            _transport = transport;
        }

        public SessionData Session => _session;

        /// <summary>Send a packet to the client (encrypt with local security).</summary>
        public void SendToClient(Packet packet) => _transport.EnqueueClient(packet);

        /// <summary>Send a packet to the server (encrypt with remote security).</summary>
        public void SendToServer(Packet packet) => _transport.EnqueueServer(packet);
    }

    /// <summary>
    /// Implemented by Module. Kept internal so filters only see SessionContext.
    /// </summary>
    internal interface ISessionTransport
    {
        void EnqueueClient(Packet packet);   // to client (S -> C)
        void EnqueueServer(Packet packet);   // to server (C -> S)
    }
}