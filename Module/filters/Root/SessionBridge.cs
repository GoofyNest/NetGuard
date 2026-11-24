using Module.Networking;

namespace NetGuard.Hot;

public static class SessionBridge
{
    public static void Attach(SessionData session) => session.Host = new SessionHost();
    public static void Sync(SessionData session) { }

    public static void Migrate(object? oldHost, object? newHost)
    {
        if (oldHost == null || newHost is not SessionHost target) return;
        // Copy shell-independent scalar state for all generated host properties,
        // including gateway flags and item cooldowns.
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
                {
                    foreach (var oldPet in pets)
                    {
                        if (oldPet == null) continue;
                        var pet = new CosState();
                        CopyScalars(oldPet, pet);
                        character.Pets.Add(pet);
                    }
                }
            }

            var inventory = Get(previous, "Inventory");
            if (inventory != null)
            {
                CopyInventory(inventory, character.Inventory);
                if (!character.Inventory.IsSynchronized)
                    InventoryTracking.LogSyncFailure(character.Inventory, character.Charname, null);
            }
            target.Agent.Chars.Add(character);
        }
    }

    private static object? Get(object source, string name) =>
        source.GetType().GetProperty(name)?.GetValue(source);

    private static void CopyInventory(object previous, Inventory target)
    {
        CopyScalars(previous, target);
        if (Get(previous, "Items") is System.Collections.IEnumerable items)
            foreach (var oldItem in items)
            {
                if (oldItem == null) continue;
                var item = new ItemState(); CopyScalars(oldItem, item);
                if (oldItem.GetType().GetProperty("Amount") == null)
                {
                    item.AmountKnown = Main.Items.TryGetValue((int)item.Id, out var record) && record.TypeID2 != 3;
                    if (!item.AmountKnown) target.Invalidate("Relog or teleport to initialize pre-update stack quantities.");
                }
                target.Put(item);
            }
        if (Get(previous, "Containers") is System.Collections.IEnumerable containers)
            foreach (var pair in containers)
            {
                var key = Get(pair!, "Key") as string; var value = Get(pair!, "Value");
                if (key == null || value == null) continue;
                var container = new Inventory(); CopyInventory(value, container); target.Containers[key] = container;
            }
        if (Get(previous, "PendingSnapshots") is System.Collections.IEnumerable snapshots)
            foreach (var pair in snapshots)
                if (Get(pair!, "Key") is string key && Get(pair!, "Value") is IEnumerable<byte> bytes)
                    target.PendingSnapshots[key] = new List<byte>(bytes);
        if (Get(previous, "NpcCodes") is System.Collections.IEnumerable npcs)
            foreach (var pair in npcs)
                if (Get(pair!, "Key") is uint id && Get(pair!, "Value") is string code) target.NpcCodes[id] = code;
        target.ItemCount = target.Items.Count;
    }

    private static void CopyScalars(object source, object destination)
    {
        foreach (var property in destination.GetType().GetProperties())
        {
            var type = property.PropertyType;
            if (!property.CanWrite || !(type.IsPrimitive || type.IsEnum || type == typeof(string)
                || type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(decimal)))
                continue;
            var old = source.GetType().GetProperty(property.Name);
            if (old != null && old.PropertyType == type)
                property.SetValue(destination, old.GetValue(source));
        }
    }
}
