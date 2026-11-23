namespace NetGuard.Hot;

public static class PetTracking
{
    public static int ItemType(ushort type) => type switch
    {
        0x08CC or 0x08CD => 1,
        0x10CC or 0x10CD => 2,
        _ => 0
    };

    public static int ObjectType(uint objectId)
    {
        if (!Main.Chars.TryGetValue((int)objectId, out var record)
            || record.TypeID1 != 1 || record.TypeID2 != 2 || record.TypeID3 != 3)
            return 0;
        return record.TypeID4 switch { 3 => 1, 4 => 2, _ => 0 };
    }

    public static void BeginSnapshot(CharState character)
    {
        lock (character.PetSync)
        {
            character.PetStateReady = false;
            character.AttackPetPending = false;
            character.GrabPetPending = false;
            character.PendingItemUses.Clear();
        }
    }

    public static void CompleteSnapshot(CharState character, List<CosState> pets)
    {
        lock (character.PetSync)
        {
            // Replace the previous world instance IDs on every login/teleport.
            character.Pets = pets;
            character.PetStateReady = true;
        }
    }

    public static bool TryUse(CharState character, int type)
    {
        lock (character.PetSync)
        {
            if (type != 0)
            {
                if (!character.PetStateReady
                    || character.Pets.Any(p => p.Type == type && p.Summoned)
                    || (type == 1 ? character.AttackPetPending : character.GrabPetPending))
                    return false;
                if (type == 1) character.AttackPetPending = true;
                else character.GrabPetPending = true;
            }
            character.PendingItemUses.Enqueue(type);
            return true;
        }
    }

    public static void UseResult(CharState character, bool success)
    {
        lock (character.PetSync)
        {
            if (character.PendingItemUses.Count == 0) return;
            int type = character.PendingItemUses.Dequeue();
            // Success is not a spawn. Keep the reservation until 0x30C8.
            if (success) return;
            if (type == 1) character.AttackPetPending = false;
            if (type == 2) character.GrabPetPending = false;
        }
    }

    public static void Spawn(CharState character, uint uniqueId, uint objectId)
    {
        lock (character.PetSync)
        {
            var pet = character.Pets.FirstOrDefault(p => p.UniqueID == uniqueId && uniqueId != 0);
            int type = ObjectType(objectId);
            // Inventory identifies the type even if characterdata is missing.
            if (type == 0)
                type = character.Pets.FirstOrDefault(p => p.ObjectID == objectId)?.Type ?? 0;
            if (type == 0 || uniqueId == 0) return; // Horses and other COS are not pets.
            pet ??= character.Pets.FirstOrDefault(p => p.ObjectID == objectId
                && p.UniqueID == 0 && p.Summoned);
            pet ??= character.Pets.FirstOrDefault(p => p.ObjectID == objectId && !p.Summoned);
            if (pet == null)
            {
                pet = new CosState();
                character.Pets.Add(pet);
            }
            pet.UniqueID = uniqueId;
            pet.ObjectID = objectId;
            pet.Type = type;
            pet.Summoned = true;
            if (type == 1) character.AttackPetPending = false;
            else character.GrabPetPending = false;
        }
    }

    public static int Destroy(CharState character, uint uniqueId)
    {
        lock (character.PetSync)
        {
            var pet = character.Pets.FirstOrDefault(p => p.UniqueID == uniqueId && uniqueId != 0);
            if (pet == null) return 0;
            pet.Summoned = false;
            // Retain the reference ID; a subsequent spawn gets a new unique ID.
            return pet.Type;
        }
    }
}
