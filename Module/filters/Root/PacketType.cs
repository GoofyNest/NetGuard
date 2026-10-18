using Module.Services;

namespace NetGuard.Filters.Shared;

/// <summary>
/// Opcode allow/deny sets for Agent-client traffic. Compiled into the hot
/// layer so edits here take effect on save — no Module.dll rebuild.
/// </summary>
public static class PacketType
{
    public static readonly HashSet<ushort> BadOpcodes =
    [
        // From your existing list — unchanged.
        0x0000, 0x3510, 0x34BB, 0x2005, 0x1005, 0x7777, 0x631D,
        0x200a, 0xa003, 0x1001, 0x1002, 0x1003, 0x1004, 0x1006,
        0x1007, 0x2311, 0x2206, 0x2209, 0x220a, 0x220e, 0x220f,
        0x6003, 0x6005, 0x6006, 0x6105, 0x620D, 0x6300, 0x6110,
        0x6207, 0x6308, 0x6312, 0x6303, 0x6905, 0x6912, 0x6315,
        0x6902, 0xa006, 0xa008, 0xa306, 0xa208, 0xa200, 0xa203,
        0x4444,
    ];

    // Lazily built on first access, so Init() is no longer needed.
    private static HashSet<ushort>? _good;
    public static HashSet<ushort> GoodOpcodes
        => _good ??= BuildGood();

    private static HashSet<ushort> BuildGood()
    {
        var s = new HashSet<ushort>();

        // Every type in this file is a ushort enum whose name matches a
        // Silkroad category. Add new enums to this list if you add them.
        foreach (var t in typeof(PacketType).Assembly.GetTypes())
        {
            if (!t.IsEnum) continue;
            if (Enum.GetUnderlyingType(t) != typeof(ushort)) continue;
            foreach (ushort v in Enum.GetValues(t))
                s.Add(v);
        }

        Custom.WriteLine($"GoodOpcodes count: {s.Count}");
        return s;
    }
}