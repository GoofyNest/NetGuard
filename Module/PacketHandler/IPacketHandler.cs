using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace Module.PacketHandler
{
    public enum PacketResultType
    {
        None,
        Block,
        Disconnect,
        [Obsolete("Persistent bans are not implemented. Use Disconnect; Ban disconnects but does not prevent reconnecting.")]
        Ban,
        SkipSending,
        DoReceive
    }

    public enum SecurityType
    {
        Default,
        RemoteSecurity,
        LocalSecurity
    }

    public interface IPacketHandler
    {
        PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx);
    }

    public class PacketList
    {
        public Packet Packet { get; set; } = null!;
        [Obsolete("All modified packets are queued immediately in list order. Use ServerReply to wait between steps.")]
        public bool SendImmediately { get; set; } = false;
        public SecurityType SecurityType { get; set; } = SecurityType.Default;
        /// <summary>For scripted server-bound packets: wait for this server opcode before the next step.</summary>
        public ushort? ServerReply { get; set; }
        /// <summary>Timeout cancels the remaining steps of this sequence. Reply payload is not parsed.</summary>
        public int ServerReplyTimeoutMs { get; set; } = 5000;
    }

    public class PacketHandlingResult : IPacketHandler
    {
        public PacketResultType ResultType { get; set; } = PacketResultType.None;
        public List<PacketList> ModifiedPackets { get; set; } = [];
        public bool SkipSending { get; set; } = false;
        PacketHandlingResult IPacketHandler.Handle(Packet packet, SessionData client, SessionContext ctx) => this;
    }
}
