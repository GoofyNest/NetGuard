namespace NetGuard.Hot;

public static class InventoryTracking
{
    public static void Apply(Packet original, SessionData client)
    {
        var character = client.CurrentChar();
        if (character == null) return;
        var inventory = character.Inventory;
        lock (inventory.Sync)
        {
            bool wasSynchronized = inventory.IsSynchronized;
            string previousError = inventory.LastError;
            bool failed = false;
            if (wasSynchronized) inventory.LastSyncLogKey = "";
            try
            {
                var packet = new Packet(original); packet.Lock();
                var next = inventory.Copy();
                ApplyPacket(packet, next, character);
                inventory.Commit(next);
            }
            catch (Exception ex)
            {
                // Never publish a partially parsed update and never swallow/modify the server reply.
                failed = true;
                inventory.Invalidate(ex.Message);
            }
            if (!inventory.IsSynchronized && (wasSynchronized || failed || inventory.LastError != previousError))
            {
                byte? operation = original.Opcode == 0xB034 && original.GetBytes().Length >= 2
                    ? original.GetBytes()[1] : null;
                LogSyncFailure(inventory, character.Charname, original.Opcode, operation);
            }
        }
    }

    public static string? TakeSyncFailureMessage(Inventory inventory, string character, ushort? opcode, byte? operation = null)
    {
        if (inventory.IsSynchronized) { inventory.LastSyncLogKey = ""; return null; }
        string context = opcode.HasValue ? $"opcode=0x{opcode:X4}" : "opcode=n/a (hot reload)";
        if (operation.HasValue) context += $" operation=0x{operation:X2}";
        string reason = string.IsNullOrWhiteSpace(inventory.LastError) ? "Inventory snapshot is incomplete." : inventory.LastError;
        string key = $"{context}|{Main.Settings.ServerType.CurrentValue}|{reason}";
        if (inventory.LastSyncLogKey == key) return null;
        inventory.LastSyncLogKey = key;
        return $"[Inventory] OUT OF SYNC character={character} {context} version={Main.Settings.ServerType.CurrentValue} reason={reason}";
    }

    public static void LogSyncFailure(Inventory inventory, string character, ushort? opcode, byte? operation = null)
    {
        string? message = TakeSyncFailureMessage(inventory, character, opcode, operation);
        if (message != null) Custom.WriteLine(message, ConsoleColor.Red);
    }

    public static void SnapshotFailed(Inventory inventory, string character, string? reason)
    {
        inventory.Invalidate(reason ?? "Inventory snapshot is incomplete.");
        LogSyncFailure(inventory, character, 0x34A6);
    }

    public static void ApplyPacket(Packet packet, Inventory inventory, CharState character)
    {
        switch (packet.Opcode)
        {
            case 0xB034: Operation(packet, inventory, character); break;
            case 0xB04C:
                if (packet.ReadUInt8() == 1) SetAmount(inventory, packet.ReadUInt8(), packet.ReadUInt16());
                break;
            case 0x3201: SetAmount(inventory, 7, packet.ReadUInt16()); break;
            case 0x3052:
            {
                int slot = packet.ReadUInt8(); uint durability = packet.ReadUInt32();
                var item = Require(inventory, slot); item.Durability = durability; inventory.Put(item); break;
            }
            case 0x3092:
            {
                byte type = packet.ReadUInt8(); byte size = packet.ReadUInt8();
                if (type == 1) inventory.Size = size;
                else if (type == 2) inventory.Container("storage").Size = size;
                break;
            }
            case 0x3040: UpdateItem(packet, inventory); break;
            case 0x3047: case 0x3253:
                packet.ReadUInt64(); inventory.PendingSnapshots[packet.Opcode == 0x3047 ? "storage" : "guild"] = new(); break;
            case 0x3049: case 0x3255:
            {
                string key = packet.Opcode == 0x3049 ? "storage" : "guild";
                if (!inventory.PendingSnapshots.TryGetValue(key, out var bytes)) throw new InvalidDataException("Storage chunk without begin.");
                if (bytes.Count + packet.GetBytes().Length > 2 * 1024 * 1024) throw new InvalidDataException("Storage snapshot too large.");
                bytes.AddRange(packet.GetBytes()); break;
            }
            case 0x3048: case 0x3254:
            {
                string key = packet.Opcode == 0x3048 ? "storage" : "guild";
                if (!inventory.PendingSnapshots.Remove(key, out var bytes)) throw new InvalidDataException("Storage end without begin.");
                var snapshot = new Packet(0); snapshot.WriteUInt8Array(bytes.ToArray()); snapshot.Lock();
                var parsed = InventoryItemReader.ReadInventory(snapshot);
                if (snapshot.RemainingRead() != 0) throw new InvalidDataException("Unparsed storage snapshot bytes.");
                inventory.Containers[key] = parsed; break;
            }
            case 0x30C8: PetSnapshot(packet, inventory); break;
            case 0x30C9:
            {
                uint pet = packet.ReadUInt32(); byte type = packet.ReadUInt8();
                if (type == 1) inventory.Containers.Remove(Pet(pet));
                else if (type == 2 && inventory.Containers.ContainsKey(Pet(pet))) inventory.Containers[Pet(pet)] = InventoryItemReader.ReadInventory(packet);
                break;
            }
            case 0x3085: case 0x3088:
                inventory.Containers.Remove("exchange-in"); inventory.Containers.Remove("exchange-out"); break;
            case 0x308C: ExchangeItems(packet, inventory, character.UniqueID); break;
            case 0x3087: CompleteExchange(inventory); break;
        }
    }

    private static string Pet(uint id) => "pet:" + id;
    private static ItemState Unknown(int slot, int amount = 1) => new() { Slot = slot, Amount = amount, IsKnown = false, AmountKnown = false };
    private static ItemState Require(Inventory inventory, int slot) => inventory.GetItem(slot)?.Copy() ?? Unknown(slot);
    public static void SetAmount(Inventory inventory, int slot, int amount)
    {
        if (amount == 0) { inventory.Remove(slot); return; }
        var item = Require(inventory, slot); item.Amount = amount; item.AmountKnown = true; inventory.Put(item);
    }
    private static void Subtract(Inventory inventory, int slot, int amount)
    {
        var item = Require(inventory, slot);
        if (!item.AmountKnown) { inventory.Put(item); return; }
        if (amount > item.Amount) throw new InvalidDataException("Removal exceeds tracked stack amount.");
        SetAmount(inventory, slot, item.Amount - amount);
    }
    public static void Move(Inventory inventory, int sourceSlot, int destinationSlot, int amount)
    {
        if (sourceSlot == destinationSlot) return;
        var source = Require(inventory, sourceSlot);
        var destination = inventory.GetItem(destinationSlot)?.Copy();
        bool stackable = source.IsKnown && Main.Items.TryGetValue((int)source.Id, out var record) && record.TypeID2 == 3 && record.MaxStack > 1;
        if (!source.AmountKnown) inventory.Invalidate("Move started from a stack with unknown quantity.");
        if (destination == null && stackable && amount != 0 && source.AmountKnown && amount < source.Amount)
        {
            source.Amount -= amount; inventory.Put(source);
            var split = source.Copy(); split.Slot = destinationSlot; split.Amount = amount; inventory.Put(split); return;
        }
        if (destination != null && stackable && destination.Id == source.Id && amount > 0
            && source.AmountKnown && destination.AmountKnown && destination.Amount + amount <= Main.Items[(int)source.Id].MaxStack)
        {
            if (amount > source.Amount) throw new InvalidDataException("Merge exceeds source quantity.");
            destination.Amount += amount; inventory.Put(destination); Subtract(inventory, sourceSlot, amount); return;
        }
        if (stackable && destination == null && source.AmountKnown && amount != 0 && amount != source.Amount)
            throw new InvalidDataException("Move quantity differs from the tracked stack.");
        inventory.Remove(sourceSlot); inventory.Remove(destinationSlot);
        source.Slot = destinationSlot; inventory.Put(source);
        if (destination != null) { destination.Slot = sourceSlot; inventory.Put(destination); }
    }
    private static void Move(Packet packet, Inventory inventory) => Move(inventory, packet.ReadUInt8(), packet.ReadUInt8(), packet.ReadUInt16());
    private static void Transfer(Packet packet, Inventory from, Inventory to)
    {
        int source = packet.ReadUInt8(), destination = packet.ReadUInt8();
        var item = Require(from, source); from.Remove(source); item.Slot = destination; to.Put(item);
    }
    private static void Pick(Packet packet, Inventory inventory)
    {
        int slot = packet.ReadUInt8();
        if (slot == 254) { packet.ReadUInt32(); return; }
        inventory.Put(InventoryItemReader.Read(packet, slot));
    }
    private static void Sell(Packet packet, Inventory inventory, Inventory root)
    {
        int slot = packet.ReadUInt8(), amount = packet.ReadUInt16(); uint npc = packet.ReadUInt32();
        int buybackSlot = packet.RemainingRead() > 0 ? packet.ReadUInt8() : 255;
        var sold = Require(inventory, slot); sold.Amount = amount;
        Subtract(inventory, slot, amount);
        if (buybackSlot != 255)
        {
            if (buybackSlot == 0) throw new InvalidDataException("Invalid buyback slot.");
            sold.Slot = buybackSlot - 1; root.Container("buyback").Put(sold);
        }
    }
    private static void Buyback(Packet packet, Inventory inventory)
    {
        int destination = packet.ReadUInt8(), source = packet.ReadUInt8(), amount = packet.ReadUInt16();
        var list = inventory.Container("buyback"); var item = Require(list, source);
        item.Slot = destination; item.Amount = amount; item.AmountKnown = true; inventory.Put(item);
        list.Remove(source);
        foreach (var remaining in list.Items) if (remaining.Slot > source) remaining.Slot--;
    }
    private static void Operation(Packet packet, Inventory inventory, CharState character)
    {
        if (packet.ReadUInt8() != 1) return;
        byte operation = packet.ReadUInt8();
        switch (operation)
        {
            case 0:
                Move(packet, inventory);
                if (packet.RemainingRead() > 0 && packet.ReadUInt8() != 0)
                {
                    if (packet.ReadUInt8() != 0) throw new InvalidDataException("Unknown secondary inventory move.");
                    Move(packet, inventory);
                }
                break;
            case 1: Move(packet, inventory.Container("storage")); break;
            case 2: Transfer(packet, inventory, inventory.Container("storage")); break;
            case 3: Transfer(packet, inventory.Container("storage"), inventory); break;
            case 4: case 5: case 10: case 11: case 12: case 13: case 32: case 33: break;
            case 6: Pick(packet, inventory); break;
            case 7: case 15: inventory.Remove(packet.ReadUInt8()); break;
            case 8: Purchase(packet, inventory, inventory, character); break;
            case 9: Sell(packet, inventory, inventory); break;
            case 14:
            {
                int slot = packet.ReadUInt8(); packet.ReadUInt8(); inventory.Put(InventoryItemReader.Read(packet, slot)); break;
            }
            case 16: Move(packet, inventory.Container(Pet(packet.ReadUInt32()))); break;
            case 17: Pick(packet, inventory.Container(Pet(packet.ReadUInt32()))); break;
            case 18: case 22:
            {
                var pet = inventory.Container(Pet(packet.ReadUInt32())); pet.Remove(packet.ReadUInt8()); break;
            }
            case 19: Purchase(packet, inventory.Container(Pet(packet.ReadUInt32())), inventory, character); break;
            case 20: Sell(packet, inventory.Container(Pet(packet.ReadUInt32())), inventory); break;
            case 24: Mall(packet, inventory); break;
            case 26: Transfer(packet, inventory.Container(Pet(packet.ReadUInt32())), inventory); break;
            case 27: Transfer(packet, inventory, inventory.Container(Pet(packet.ReadUInt32()))); break;
            case 28: packet.ReadUInt32(); Pick(packet, inventory); break;
            case 29: Move(packet, inventory.Container("guild")); break;
            case 30: Transfer(packet, inventory, inventory.Container("guild")); break;
            case 31: Transfer(packet, inventory.Container("guild"), inventory); break;
            case 34: Buyback(packet, inventory); break;
            case 35: case 36:
            {
                var avatar = inventory.Container("avatar");
                var from = operation == 35 ? avatar : inventory; var to = operation == 35 ? inventory : avatar;
                Transfer(packet, from, to); packet.ReadUInt16();
                if (packet.RemainingRead() > 0 && packet.ReadUInt8() != 0)
                {
                    byte direction = packet.ReadUInt8();
                    if (direction != (operation == 35 ? 35 : 0)) throw new InvalidDataException("Unknown secondary avatar move.");
                    Transfer(packet, from, to);
                }
                break;
            }
            case 43:
                Purchase(packet, inventory, inventory, character);
                packet.ReadUInt8(); SetAmount(inventory, packet.ReadUInt8(), packet.ReadUInt16()); packet.ReadUInt32(); break;
            case 47: Transfer(packet, inventory, inventory.Container("job")); packet.ReadUInt16(); break;
            case 48: Transfer(packet, inventory.Container("job"), inventory); packet.ReadUInt16(); break;
            case 44: case 46: case 51: break; // Separate specialty-bag slot space, not player inventory.
            default: throw new InvalidDataException($"Unsupported inventory operation 0x{operation:X2}.");
        }
    }
    private static void UpdateItem(Packet packet, Inventory inventory)
    {
        int slot = packet.ReadUInt8(), flags = packet.ReadUInt8(); var item = Require(inventory, slot);
        if ((flags & 1) != 0)
        {
            item.Id = packet.ReadUInt32();
            item.IsKnown = Main.Items.TryGetValue((int)item.Id, out var record);
            item.CodeName128 = record?.CodeName128 ?? "";
        }
        if ((flags & 2) != 0) item.OptLevel = packet.ReadUInt8();
        if ((flags & 4) != 0) item.Variance = packet.ReadUInt64();
        if ((flags & 8) != 0) { item.Amount = packet.ReadUInt16(); item.AmountKnown = true; }
        if ((flags & 16) != 0) item.Durability = packet.ReadUInt32();
        if ((flags & 64) != 0) item.State = packet.ReadUInt8();
        if ((flags & 32) != 0) { item.MagParamNum = packet.ReadUInt8(); InventoryItemReader.SkipMagic(packet, item.MagParamNum); }
        inventory.Put(item);
    }
    private static void PetSnapshot(Packet packet, Inventory inventory)
    {
        uint id = packet.ReadUInt32(), reference = packet.ReadUInt32();
        if (!Main.Chars.TryGetValue((int)reference, out var record) || record.TypeID2 != 2 || record.TypeID3 != 3) return;
        if (record.TypeID4 != 2 && record.TypeID4 != 4) return;
        packet.ReadUInt32(); packet.ReadUInt32();
        if (record.TypeID4 == 4) { packet.ReadUInt32(); packet.ReadAscii(); }
        var pet = InventoryItemReader.ReadInventory(packet);
        packet.ReadUInt32(); // owner
        if (record.TypeID4 == 4) packet.ReadUInt8();
        inventory.Containers[Pet(id)] = pet;
    }
    private static void ExchangeItems(Packet packet, Inventory inventory, uint self)
    {
        bool outgoing = packet.ReadUInt32() == self; int count = packet.ReadUInt8(); var list = new Inventory();
        for (int i = 0; i < count; i++)
        {
            int source = outgoing ? packet.ReadUInt8() : -1; int exchangeSlot = packet.ReadUInt8();
            list.Put(InventoryItemReader.Read(packet, outgoing ? source : exchangeSlot));
        }
        inventory.Containers[outgoing ? "exchange-out" : "exchange-in"] = list;
    }
    private static void CompleteExchange(Inventory inventory)
    {
        // Follow the RSBot exchange completion order: incoming slots are allocated first.
        if (inventory.Containers.Remove("exchange-in", out var incoming))
            foreach (var item in incoming.Items)
            {
                int free = Enumerable.Range(13, Math.Max(0, inventory.Size - 13)).FirstOrDefault(s => inventory.GetItem(s) == null, -1);
                if (free < 0) throw new InvalidDataException("No free slot for completed exchange.");
                item.Slot = free; inventory.Put(item);
            }
        if (inventory.Containers.Remove("exchange-out", out var outgoing))
            foreach (var item in outgoing.Items) Subtract(inventory, item.Slot, item.Amount);
    }
    private static void Purchase(Packet packet, Inventory target, Inventory root, CharState character)
    {
        EnsurePurchaseVersion();
        int tab = packet.ReadUInt8(), goodsSlot = packet.ReadUInt8(), count = packet.ReadUInt8();
        var slots = new int[count]; for (int i = 0; i < count; i++) slots[i] = packet.ReadUInt8();
        int amount = packet.ReadUInt16();
        var template = InventoryShops.ResolveNpc(character.SelectedNpcCode, tab, goodsSlot);
        foreach (int slot in slots)
        {
            var item = template.Copy(); item.Slot = slot;
            item.Amount = InventoryShops.IsStackable(item.Id) ? amount : 1;
            target.Put(item);
        }
    }
    private static void Mall(Packet packet, Inventory inventory)
    {
        EnsurePurchaseVersion();
        int group = packet.ReadUInt16(), shop = packet.ReadUInt8(), tab = packet.ReadUInt8(), good = packet.ReadUInt8(), count = packet.ReadUInt8();
        var slots = new int[count]; for (int i = 0; i < count; i++) slots[i] = packet.ReadUInt8();
        int amount = packet.ReadUInt16(); var template = InventoryShops.ResolveMall(group, shop, tab, good);
        foreach (int slot in slots)
        {
            var item = template.Copy(); item.Slot = slot; item.Amount = InventoryShops.IsStackable(item.Id) ? Math.Max(1, amount) : 1;
            // Cash-shop rental metadata follows each item on vSRO.
            if (InventoryItemReader.Vsro) InventoryItemReader.SkipRental(packet);
            inventory.Put(item);
        }
    }
    private static void EnsurePurchaseVersion()
    {
        if (Main.Settings.ServerType.CurrentValue != "jSRO" && !InventoryItemReader.Vsro)
            throw new NotSupportedException("Shop reply layout currently supports jSRO/vSRO only.");
    }
}
