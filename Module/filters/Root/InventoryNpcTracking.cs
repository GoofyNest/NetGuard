namespace NetGuard.Hot;

public static class InventoryNpcTracking
{
    public static void Apply(Packet original, SessionData client)
    {
        var character = client.CurrentChar(); if (character == null) return;
        var inventory = character.Inventory;
        lock (inventory.Sync)
        {
            try
            {
                var packet = new Packet(original); packet.Lock();
                switch (packet.Opcode)
                {
                    case 0xB045:
                        if (packet.ReadUInt8() == 1)
                        {
                            uint id = packet.ReadUInt32();
                            string next = inventory.NpcCodes.GetValueOrDefault(id, "");
                            if (id != character.SelectedNpcUniqueID) inventory.Containers.Remove("buyback");
                            character.SelectedNpcCode = next;
                            character.SelectedNpcUniqueID = id;
                        }
                        break;
                    case 0x3015: ReadNpc(packet, inventory, false); break;
                    case 0x3016: inventory.NpcCodes.Remove(packet.ReadUInt32()); break;
                    case 0x3017:
                    {
                        var header = packet.GetBytes(); inventory.PendingSnapshots["npc-spawns"] = new List<byte>(header); break;
                    }
                    case 0x3019:
                        if (inventory.PendingSnapshots.TryGetValue("npc-spawns", out var chunkBytes))
                        {
                            if (chunkBytes.Count + packet.GetBytes().Length > 2 * 1024 * 1024) throw new InvalidDataException("Spawn group too large.");
                            chunkBytes.AddRange(packet.GetBytes());
                        }
                        break;
                    case 0x3018:
                    {
                        if (!inventory.PendingSnapshots.Remove("npc-spawns", out var bytes)) break;
                        var group = new Packet(0); group.WriteUInt8Array(bytes.ToArray()); group.Lock();
                        byte type = group.ReadUInt8(); int count = group.ReadUInt16();
                        for (int i = 0; i < count; i++)
                        {
                            if (type == 2) inventory.NpcCodes.Remove(group.ReadUInt32());
                            else if (type != 1 || !ReadNpc(group, inventory, true)) break;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex) { Custom.WriteLine($"[Inventory NPC] 0x{original.Opcode:X4}: {ex.Message}", ConsoleColor.Red); }
        }
    }
    // Only consume layouts whose boundaries are known. Never scan arbitrary payload bytes for NPC IDs.
    private static bool ReadNpc(Packet packet, Inventory inventory, bool group)
    {
        uint reference = packet.ReadUInt32();
        if (!Main.Chars.TryGetValue((int)reference, out var record)
            || record.TypeID1 != 1 || record.TypeID2 != 2 || record.TypeID3 != 2) return false;
        uint unique = packet.ReadUInt32();
        if (!group) { inventory.NpcCodes[unique] = record.CodeName128; return true; }
        packet.ReadUInt16(); packet.ReadFloat(); packet.ReadFloat(); packet.ReadFloat(); packet.ReadUInt16();
        bool moving = packet.ReadUInt8() != 0; packet.ReadUInt8();
        if (moving)
        {
            ushort region = packet.ReadUInt16();
            if ((region & 0x8000) != 0) { packet.ReadUInt32(); packet.ReadUInt32(); packet.ReadUInt32(); }
            else { packet.ReadUInt16(); packet.ReadUInt16(); packet.ReadUInt16(); }
        }
        else { packet.ReadUInt8(); packet.ReadUInt16(); }
        packet.ReadUInt8(); packet.ReadUInt8(); packet.ReadUInt8();
        if (InventoryItemReader.Vsro) packet.ReadUInt8();
        packet.ReadFloat(); packet.ReadFloat(); packet.ReadFloat();
        int buffs = packet.ReadUInt8();
        for (int i = 0; i < buffs; i++)
        {
            uint skill = packet.ReadUInt32(); packet.ReadUInt32();
            if (!Main.Skills.TryGetValue((int)skill, out var data)) throw new InvalidDataException("Unknown NPC buff layout.");
            if (data.ParamsContains(1701213281)) packet.ReadUInt8();
        }
        byte talk = packet.ReadUInt8();
        if ((talk & 2) != 0) packet.ReadUInt8Array(InventoryItemReader.Vsro ? packet.ReadUInt8() : 4);
        if (talk == 6 && packet.ReadUInt8() == 1) packet.ReadUInt32();
        inventory.NpcCodes[unique] = record.CodeName128;
        return true;
    }
}
