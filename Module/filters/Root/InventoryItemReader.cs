namespace NetGuard.Hot;

// Matches the existing CharData item reads for this proxy's jSRO/vSRO layouts.
// This parser is for server inventory deltas and auxiliary inventories only.
public static class InventoryItemReader
{
    public static bool Vsro => Main.Settings.ServerType.CurrentValue == "vSRO";
    public static ItemState Read(Packet packet, int slot)
    {
        if (Main.Settings.ServerType.CurrentValue != "jSRO" && !Vsro)
            throw new NotSupportedException("Live item decoding currently supports the existing jSRO/vSRO layouts only.");
        var item = new ItemState { Slot = slot };
        if (Vsro) SkipRental(packet);
        item.Id = packet.ReadUInt32();
        if (!Main.Items.TryGetValue((int)item.Id, out var record))
            throw new InvalidDataException($"Unknown item reference {item.Id}.");
        item.CodeName128 = record.CodeName128;
        if (record.TypeID2 == 1 || record.TypeID2 == 4 || record.TypeID2 == 5)
        {
            item.OptLevel = packet.ReadUInt8(); item.Variance = packet.ReadUInt64();
            item.Durability = packet.ReadUInt32(); item.MagParamNum = packet.ReadUInt8();
            SkipMagic(packet, item.MagParamNum);
            if (Vsro)
                for (int group = 0; group < 2; group++)
                {
                    int count = packet.ReadUInt8(); packet.ReadUInt8();
                    for (int i = 0; i < count; i++) { packet.ReadUInt8(); packet.ReadUInt32(); packet.ReadUInt32(); }
                }
        }
        else if (record.TypeID2 == 2 && record.TypeID3 == 1)
        {
            item.State = packet.ReadUInt8();
            if (item.State == 2 || item.State == 3 || item.State == 4)
            {
                packet.ReadUInt32(); packet.ReadAscii();
                if (record.TypeID4 == 2)
                {
                    if (!Vsro) packet.ReadUInt32();
                    else if (packet.ReadUInt8() == 1)
                    {
                        packet.ReadUInt8(); packet.ReadUInt32();
                        if (packet.ReadUInt32() == 5) { packet.ReadUInt32(); packet.ReadUInt8(); }
                    }
                }
            }
        }
        else if (record.TypeID2 == 2 && (record.TypeID3 == 2 || record.TypeID3 == 3)) packet.ReadUInt32();
        else if (record.TypeID2 == 3 && record.TypeID3 == 8 && record.TypeID4 == 3) packet.ReadUInt32();
        else if (record.TypeID2 == 3)
        {
            item.Amount = packet.ReadUInt16();
            if (record.TypeID3 == 11 && (record.TypeID4 == 1 || record.TypeID4 == 2)) packet.ReadUInt8();
            else if (record.TypeID3 == 14 && record.TypeID4 == 2) SkipMagic(packet, packet.ReadUInt8());
        }
        return item;
    }
    public static void SkipMagic(Packet packet, int count)
    {
        for (int i = 0; i < count; i++) { packet.ReadUInt32(); packet.ReadUInt32(); }
    }
    public static void SkipRental(Packet packet)
    {
        uint rental = packet.ReadUInt32();
        switch (rental)
        {
            case 0: break;
            case 1: packet.ReadUInt16(); packet.ReadUInt32(); packet.ReadUInt32(); break;
            case 2: packet.ReadUInt16(); packet.ReadUInt16(); packet.ReadUInt32(); break;
            case 3: packet.ReadUInt16(); packet.ReadUInt16(); packet.ReadUInt32(); packet.ReadUInt32(); packet.ReadUInt32(); break;
            default: throw new InvalidDataException($"Unknown rental type {rental}.");
        }
    }
    public static Inventory ReadInventory(Packet packet)
    {
        var inventory = new Inventory { Size = packet.ReadUInt8() };
        if (inventory.Size == 0) return inventory;
        int count = packet.ReadUInt8();
        for (int i = 0; i < count; i++) inventory.Put(Read(packet, packet.ReadUInt8()));
        if (inventory.Items.Count != count) throw new InvalidDataException("Duplicate or empty slots in inventory snapshot.");
        return inventory;
    }
}
