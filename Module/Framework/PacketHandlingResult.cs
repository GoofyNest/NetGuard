﻿using SilkroadSecurityAPI;

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

    public class PacketList
    {
        public required Packet Packet { get; set; }
        public bool SendImmediately { get; set; } = false; // Whether Send(true) or Send(false)
        public SecurityType SecurityType { get; set; } = SecurityType.Default; // Whether to use _remoteSecurity.Send or _localSecurity.Send to spoof a packet
    }

    public class PacketHandlingResult
    {
        public PacketResultType ResultType { get; set; } = PacketResultType.None;
        public List<PacketList> ModifiedPackets { get; set; } = [];
        public bool SkipSending { get; set; } = false;

    }
}
