using SilkroadSecurityAPI;

namespace Module.Framework
{
    public enum PacketResultType
    {
        None,         // Do nothing, default
        Block,        // Block further processing
        Disconnect,   // Disconnect the client
        Ban,          // Ban the client
        SkipSending,   // Whether avoiding using _localSecurity.Send or _remoteSecurity.Send
        DoReceive
    }

    public enum SecurityType
    {
        Default,
        RemoteSecurity,
        LocalSecurity
    }

    public class PacketHandlingResult
    {
        public PacketResultType ResultType { get; set; } = PacketResultType.None;
        public Packet ModifiedPacket { get; set; } = null!;
        public bool SendImmediately { get; set; } = false; // Whether Send(true) or Send(false)
        public SecurityType securityType { get; set; } = SecurityType.Default; // Whether to use _remoteSecurity.Send or _localSecurity.Send to spoof a packet
    }
}
