namespace NetGuard.Hot;

public static class PacketDiagnostics
{
    // Set NETGUARD_PACKET_DUMPS=1 before starting NetGuard to enable dumps.
    public static bool Enabled { get; set; } = Environment.GetEnvironmentVariable("NETGUARD_PACKET_DUMPS") == "1";
}
