# NetGuard

NetGuard is a C# packet filter for `Legend 1` Silkroad Online files, with potential compatibility for vSRO 1.88.

**Hot-reloadable filters. No rebuilds, restarts, or player disconnects.**

## Getting Started

1. **Download** — Clone the repository or download the latest build from the [Releases](../../releases) page.
2. **Run** — Launch `NetGuardLoader.exe`. It automatically generates the required configuration files and `.bat` files for running the modules.
3. **Configure** — Set up your module bindings, IP addresses, ports, and API keys in the generated configuration files.
4. **Launch** — Use the generated `.bat` files to start the Gateway and Agent modules.
5. **Filter** — Add or edit `.cs` files under `filters/`. Save your changes and NetGuard automatically reloads the filters.

## Features

- **Hot Reloading** — Compile and swap filters at runtime without restarting the proxy.
- **Per-Context Filtering** — Separate filtering contexts for Agent and Gateway, handling both client and server traffic.
- **Landing Layer** — Centralized packet validation and state-dependent filtering before opcode dispatch.
- **SessionBus** — HMAC-SHA256-authenticated communication between modules, enabling shared player state across machines.
- **Synchronous Handlers** — No `async` / `await` in filter handlers, helping minimize per-call overhead.
- **Cross-Platform** — Built on .NET 10 for Windows and Linux.
- **Open Source** — Free to use and modify, with no artificial player caps or hosting restrictions.

## Performance Preview

[Watch the performance preview on YouTube](https://www.youtube.com/watch?v=u4uCDbXQ3rw)

## Quick Example

Filters are organized by module, direction, and feature:

```text
filters/
└── Agent/
    └── Client/
        └── Feature/
            └── BlockSuspiciousPickup.cs
```

Example filter:

```csharp
using Module.Networking;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace NetGuard.Filters.Agent.Client;

[Filter(0x7074)] // AGENT_PLAYER_ACTION
public sealed class BlockSuspiciousPickup : IFilter
{
    public PacketHandlingResult Handle(
        Packet packet,
        SessionData client,
        SessionContext ctx)
    {
        if (!client.Agent.IsIngame)
        {
            return new PacketHandlingResult
            {
                ResultType = PacketResultType.Block
            };
        }

        return new PacketHandlingResult();
    }
}
```

## Development & Support

NetGuard is actively developed, with compatibility improvements and new features added over time.

Need a custom feature or help with setup? https://github.com/GoofyNest/NetGuard/issues

## Acknowledgements

- [DaxterSoul](https://www.elitepvpers.com/forum/members/1084164-daxtersoul.html) — [SilkroadDocs](https://github.com/DummkopfOfHachtenduden/SilkroadDoc/).
- [pushedx](https://www.elitepvpers.com/forum/sro-coding-corner/1063078-c-silkroadsecurity.html) — SilkroadSecurityAPI.
- **Chernobyl** — The original packet filter concept and foundation.
- [DuckSoup](https://github.com/ducksoup-sro/ducksoup) — Packet parsing contributions.

## License

Do what you want, 'cause a pirate is free.
