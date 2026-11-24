# Live player inventory

`client.CurrentChar()?.Inventory` is updated from successful server replies. `Items` contains one entry per slot; `ItemCount` is the occupied-slot count and `ItemState.Amount` is the stack quantity. Failed server operations do not change the inventory.

Updates are parsed into a copy and committed together. A truncated or unsupported packet leaves the previous data intact, sets `IsSynchronized = false`, and records `LastError`. Do not use an unsynchronized inventory for security decisions. A successful CharData inventory snapshot resets it. Previously connected players need a teleport/relog after the first installation because their old stack quantities were discarded.

Out-of-sync diagnostics use `[Inventory] OUT OF SYNC` and include character, opcode, game version, and reason. For `0xB034`, the operation byte is included too. The same consecutive failure is suppressed; a new reason or a new synchronization-loss episode is logged again. Snapshot failures identify `0x34A6`; migration failures say `opcode=n/a (hot reload)` because no packet caused them.

## Packets to test

| Opcode | Changes tracked |
|---|---|
| 0xB034 | Moves, swaps, splits, merges, pickups, drops, server additions/removals, NPC sales/buyback, purchases, storage/guild/pet/avatar/job transfers |
| 0xB04C | Remaining quantity after item use; existing pet/cooldown logic retained |
| 0x3040 | Item ID, plus level, variance, quantity, durability, state, magic-option count |
| 0x3052 / 0x3201 / 0x3092 | Durability, ammunition, inventory capacity |
| 0x3047/49/48 and 0x3253/55/54 | Personal/guild storage snapshots |
| 0x30C8/0x30C9 | Pet inventory snapshots and removal |
| 0x3085/87/88/8C | Exchange contents, completion, cancellation |

## Game-version checks

The setting remains `Main.Settings.ServerType.CurrentValue`. New full-item/purchase decoding supports the existing `jSRO` and `vSRO` layouts. Other versions are reported as unsupported rather than interpreted as jSRO.

- `Root/InventoryItemReader.cs`: vSRO rental prefix, equipment binding groups, pet-item rental fields. Its reads match the existing CharData item parser. jSRO omits those prefixes/groups and uses its existing pet expiry field.
- `Root/InventoryTracking.cs`: vSRO cash-shop rental suffix. NPC purchase replies use tab, goods slot, destination count/slots, then amount, for these supported versions.
- `Root/InventoryNpcTracking.cs`: vSRO NPC body-state byte and talk-option count; jSRO uses the older NPC layout.
- CharData retains exactly the same read calls and order. Existing stack quantities and pet states are now stored rather than discarded; snapshot success is recorded.

## Limits to verify live

NPC purchases need the selected NPC's reference identity. It is cached from single NPC spawns and NPC entries at the start of spawn groups. Group parsing stops at a non-NPC entry whose size this helper does not know; it never guesses byte offsets. A purchase without a known NPC identity is flagged out of sync. Shop reference TXT files are read from the configured client_data directory, including both mapping files.

Transfers from a container that has not been observed retain an explicitly unknown item (`IsKnown = false`) and invalidate synchronization. Storage and grab-pet snapshots populate those containers. Avatar/job inventory contents are not added to the CharData parser; a first transfer from an unseen avatar/job slot therefore needs a subsequent snapshot or item update to identify it.

Exchange destination slots follow the RSBot first-free-slot rule, allocating incoming items before outgoing removals; verify this against your server. Unrecognized inventory operation subtypes are flagged, not guessed. New snapshot layouts and shop purchases still need live-packet testing for your client version.

All registered server handlers have exception guards. Landing.cs files are unchanged. No packet is rewritten by the inventory observer.
