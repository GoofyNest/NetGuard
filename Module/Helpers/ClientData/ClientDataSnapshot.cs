using System.Collections.Concurrent;

namespace Module.Helpers.ClientData;

public sealed class ClientDataSnapshot
{
    public ConcurrentDictionary<int, ItemData> Items { get; } = new();
    public ConcurrentDictionary<int, SkillData> Skills { get; } = new();
    public ConcurrentDictionary<int, CharData> Chars { get; } = new();
}
