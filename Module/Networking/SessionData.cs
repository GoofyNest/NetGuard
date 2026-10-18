using SilkroadSecurityAPI;

namespace Module.Networking
{
    /// <summary>
    /// Host-visible shell. ONLY fields that compiled Module.dll code reads
    /// belong here. Everything else lives in the hot-loaded SessionHost.
    /// </summary>
    public class SessionData
    {
        public PlayerInformation PlayerInfo { get; set; } = new();
        public GatewayServer Gateway { get; set; } = new();
        public AgentServer Agent { get; set; } = new();

        /// <summary>Instance of NetGuard.Hot.SessionHost, set by FilterLoader.</summary>
        public object? Host { get; set; }
    }

    public class PlayerInformation
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Mac { get; set; } = "";
        public ushort ServerID { get; set; }
        public string CurrentCharName { get; set; } = string.Empty;
        public IPInformation IpInfo { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class IPInformation
    {
        public string Ip { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>Empty shell. Filters use client.Host().Gateway.</summary>
    public class GatewayServer 
    {
        public bool SentPatchResponse { get; set; }
        public bool SentShardListResponse { get; set; }
        public bool SentLoginRequest { get; set; }
    }

    /// <summary>
    /// Only the three flags PacketHandler.cs reads live here.
    /// Everything else (CharData, CharDataProcess, etc.) lives in Host.Agent.
    /// </summary>
    public class AgentServer
    {
        public bool LoginRequested { get; set; }
        public bool IsIngame { get; set; }
        public bool InCharSelectionScreen { get; set; }
        public bool SentJoinRequest { get; set; }
        public bool ClientQuitGame { get; set; }
        public byte LeaveType { get; set; }

        // NEW — set by your exchange-related filters; read by the Landing.
        public bool InExchange { get; set; }
    }
}