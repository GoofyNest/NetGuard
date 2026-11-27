using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetGuard.DesktopProtocol;

public static class Protocol
{
    public const string Prefix = "@NETGUARD:";
    public const int MaximumPayload = 64 * 1024;
    public static readonly JsonSerializerOptions Json = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
}

public sealed record CaptureOptions
{
    public bool Enabled { get; init; }
    public string? Ip { get; init; }
    public bool ClientToServer { get; init; } = true;
    public bool ServerToClient { get; init; } = true;
    public ushort[] Ignore { get; init; } = [];
    public ushort[] Only { get; init; } = [];
}

public sealed record InjectionRequest(int ClientId, bool ToServer, ushort Opcode, byte[] Payload, bool Encrypted, bool Massive);
public sealed record DesktopCommand(string Type, CaptureOptions? Capture = null, InjectionRequest? Injection = null);
public sealed record PeerInfo(int ClientId, string Ip);
public sealed record DesktopMessage
{
    public string Type { get; init; } = "packet";
    public string Stage { get; init; } = "received";
    public int Module { get; init; }
    public DateTimeOffset Time { get; init; } = DateTimeOffset.Now;
    public int ClientId { get; init; }
    public string? Ip { get; init; }
    public bool FromClient { get; init; }
    public ushort Opcode { get; init; }
    public byte[]? Payload { get; init; }
    public int Length { get; init; }
    public bool Encrypted { get; init; }
    public bool Massive { get; init; }
    public PeerInfo[]? Peers { get; init; }
    public string? Text { get; init; }
    public long Dropped { get; init; }
}
