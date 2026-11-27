using Module.Networking;

namespace NetGuard.Hot;

public static class SessionBridge
{
    public static void Attach(SessionData session) => session.Host = new SessionHost();
    public static void Sync(SessionData session) { }

    public static void Migrate(object? oldHost, object? newHost)
    {
        if (oldHost == null || newHost is not SessionHost target) return;

        // Preserve scalar state (including cooldowns and teleport flags) without
        // carrying references to types from the previous filter assembly.
        foreach (var property in typeof(SessionHost).GetProperties())
        {
            var previous = Get(oldHost, property.Name);
            var next = property.GetValue(target);
            if (previous != null && next != null) CopyScalars(previous, next);
        }

        var agent = Get(oldHost, "Agent");
        if (agent == null) return;
        if (Get(agent, "CharData") is SilkroadSecurityAPI.Packet packet)
            target.Agent.CharData = new SilkroadSecurityAPI.Packet(packet);
        if (Get(agent, "SpawnedPets") is System.Collections.IEnumerable spawned)
            foreach (var id in spawned) target.Agent.SpawnedPets.Add(Convert.ToUInt32(id));

        if (Get(agent, "Chars") is not System.Collections.IEnumerable characters) return;
        foreach (var previous in characters)
        {
            if (previous == null) continue;
            var character = new CharState();
            lock (Get(previous, "PetSync") ?? previous)
            {
                CopyScalars(previous, character);
                if (Get(previous, "PendingItemUses") is System.Collections.IEnumerable pending)
                    foreach (var type in pending) character.PendingItemUses.Enqueue(Convert.ToInt32(type));
                if (Get(previous, "Pets") is System.Collections.IEnumerable pets)
                    foreach (var previousPet in pets)
                    {
                        if (previousPet == null) continue;
                        var pet = new CosState();
                        CopyScalars(previousPet, pet);
                        character.Pets.Add(pet);
                    }
            }
            // Inventory deliberately remains empty: only CharData populates it.
            target.Agent.Chars.Add(character);
        }
    }

    private static object? Get(object source, string name)
        => source.GetType().GetProperty(name)?.GetValue(source);

    // Weak source keys avoid retaining previous collectible filter assemblies.
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Type,
        System.Collections.Concurrent.ConcurrentDictionary<Type, (System.Reflection.PropertyInfo Source, System.Reflection.PropertyInfo Target)[]>> Maps = new();

    private static void CopyScalars(object source, object target)
    {
        var targets = Maps.GetValue(source.GetType(), static _ => new());
        var pairs = targets.GetOrAdd(target.GetType(), targetType =>
        {
            var properties = new List<(System.Reflection.PropertyInfo, System.Reflection.PropertyInfo)>();
            foreach (var property in targetType.GetProperties())
            {
                if (!property.CanWrite || property.GetIndexParameters().Length != 0) continue;
                var type = property.PropertyType;
                if (!(type.IsPrimitive || type.IsEnum || type == typeof(string)
                    || type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(decimal))) continue;
                var previous = source.GetType().GetProperty(property.Name);
                if (previous?.CanRead == true && previous.PropertyType == type)
                    properties.Add((previous, property));
            }
            return properties.ToArray();
        });
        foreach (var (previous, property) in pairs)
            property.SetValue(target, previous.GetValue(source));
    }
}
