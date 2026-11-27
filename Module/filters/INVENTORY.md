# Inventory snapshots

Player inventory is populated only by `Agent/Server/Character/CharacterDataEndResponse.cs` from the CharData network packet. Its packet fields and read order are unchanged.

Inventory is a login/teleport snapshot. Moving, buying, selling, dropping, consuming and exchanging items do not update it. It must not be treated as a live inventory for protection decisions.

Hot reload leaves inventory empty until the next CharData snapshot. SessionBridge preserves character scalar state, pet records and pending summons, cooldowns, teleport flags and an in-progress CharData packet; it does not populate inventory.

Pet protection is independent: `0x704C` allows one attack pet and one grab pet, `0xB04C` handles pending use results, and `0x30C8`/`0x30C9` update pet spawn/despawn state. None of these packets writes inventory.

Validation: `dotnet run --project tests/NetGuard.Snapshot.Tests/NetGuard.Snapshot.Tests.csproj`.
