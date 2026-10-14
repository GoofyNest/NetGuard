# NetGuard Dynamic Filters

Each `.cs` file in this tree is compiled at runtime by NetGuard and hot-reloaded on save.

## Folder = context

    filters/Agent/Client/     C -> S, agent module
    filters/Agent/Server/     S -> C, agent module
    filters/Gateway/Client/   C -> S, gateway module
    filters/Gateway/Server/   S -> C, gateway module

The folder you drop a file into determines which dispatcher sees it. The same
opcode can exist in different folders (different context). The same opcode
cannot exist twice in the *same* folder — the second one is rejected.

## Rules

1. **Filename must match class name.** `CharAction.cs` -> `class CharAction`.
2. **Class must implement `IFilter`** and have a parameterless constructor.
3. **Class must have `[Filter(opcode)]`** where `opcode` is the packet opcode.
4. **One opcode per file per folder.**
5. **Save the file — reload happens ~750ms later.**

## Example

    using Module.Networking;
    using Module.PacketHandler;
    using Module.PacketHandler.Filtering;
    using SilkroadSecurityAPI;

    namespace MyFilters
    {
        [Filter(0x7074)]
        public sealed class CharAction : IFilter
        {
            public PacketHandlingResult Handle(Packet packet, SessionData client)
            {
                // return new PacketHandlingResult() to forward unchanged
                // return new PacketHandlingResult { ResultType = PacketResultType.Block } to drop
                // return with ModifiedPackets to replace
                return new PacketHandlingResult();
            }
        }
    }

## What you can reference

Any assembly loaded by NetGuard at runtime: `SilkroadSecurityAPI`, `Newtonsoft.Json`,
all your `Module.*` types (`SessionData`, `PacketHandlingResult`, `PacketList`, enums, etc.).

## What you cannot do

- Use `async`/`await` in `Handle` (signature is sync).
- Reference assemblies not loaded by the host at compile time.
- Add NuGet packages (compile is offline, using only host-loaded assemblies).