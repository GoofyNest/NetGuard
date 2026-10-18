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
        public bool SendImmediately { get; set; } = false;
        public SecurityType SecurityType { get; set; } = SecurityType.Default;
    }

    public class PacketHandlingResult
    {
        public PacketResultType ResultType { get; set; } = PacketResultType.None;
        public List<PacketList> ModifiedPackets { get; set; } = [];
        public bool SkipSending { get; set; } = false;
    }
}