using System.Text.Json;
using Module.Networking;
using NetGuard.DesktopProtocol;
using SilkroadSecurityAPI;

internal static class DesktopDiagnosticsTests
{
    internal static async Task<int> Run()
    {
        int checks = 0;
        void Check(bool value, string name) { if (!value) throw new Exception(name); checks++; }
        var output = new StringWriter();
        var diagnostics = new DesktopDiagnostics(2, output);
        var packet = new Packet(0x7074, true); packet.WriteUInt8Array([1, 2, 3]); packet.Lock();
        packet.ReadUInt8();
        diagnostics.Capture(packet, "127.0.0.1", 5, true); // disabled
        diagnostics.Configure(new() { Enabled = true, Ip = "127.0.0.1", Only = [0x7074], ServerToClient = false });
        diagnostics.Capture(packet, "127.0.0.2", 5, true); // wrong IP
        diagnostics.Capture(packet, "127.0.0.1", 5, false); // wrong direction
        diagnostics.Capture(new Packet(0x7075), "127.0.0.1", 5, true); // wrong opcode
        diagnostics.Capture(packet, "127.0.0.1", 5, true);
        Check(packet.ReadUInt8() == 2, "Capture never advances the packet reader");
        diagnostics.Configure(new() { Enabled = true, Only = [0x7074], Ignore = [0x7074] });
        diagnostics.Capture(packet, "127.0.0.1", 5, true); // ignore wins
        diagnostics.Configure(new() { Enabled = true });
        var large = new Packet(0x3013, false, true, new byte[Protocol.MaximumPayload + 10]);
        diagnostics.Capture(large, "127.0.0.1", 5, false);
        int calls = 0; bool? destination = null; byte[]? payload = null;
        diagnostics.Register(5, (injected, toServer, _) =>
        { calls++; destination = toServer; payload = injected.CopyPayload(100); return Task.CompletedTask; });
        async Task Inject(int id, bool remote, ushort opcode) => await diagnostics.HandleCommandAsync(JsonSerializer.Serialize(
            new DesktopCommand("inject", Injection: new(id, remote, opcode, [9, 8], true, false)), Protocol.Json));
        await Inject(5, true, 0x7074);
        Check(calls == 1 && destination == true && payload!.SequenceEqual(new byte[] { 9, 8 }), "Remote injection routes payload to server transport");
        await Inject(5, false, 0x7074);
        Check(calls == 2 && destination == false, "Local injection routes to client transport");
        await Inject(5, true, 0x5000); await Inject(5, true, 0x9000); await Inject(5, true, 0x600D);
        Check(calls == 2, "Transport opcodes cannot be injected");
        diagnostics.Unregister(5);
        await Inject(5, true, 0x7074);
        Check(calls == 2, "Disconnected injection target cannot be used");
        await diagnostics.HandleCommandAsync("{broken");
        await diagnostics.DisposeAsync();
        var messages = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => JsonSerializer.Deserialize<DesktopMessage>(line[Protocol.Prefix.Length..], Protocol.Json)!).ToArray();
        var packets = messages.Where(m => m.Type == "packet").ToArray();
        Check(packets.Length == 2 && packets[0].Payload!.SequenceEqual(new byte[] { 1, 2, 3 }), "IP, direction and opcode selection applied before capture");
        Check(packets[1].Payload!.Length == Protocol.MaximumPayload && packets[1].Length == Protocol.MaximumPayload + 10,
            "Large captures report original length and bounded payload");
        Check(messages.Count(m => m.Type == "injection") == 6, "Each injection receives a result");

        var blocked = new BlockedWriter();
        var congested = new DesktopDiagnostics(0, blocked);
        congested.Configure(new() { Enabled = true });
        congested.Capture(packet, "127.0.0.1", 1, true);
        await blocked.Entered.Task.WaitAsync(TimeSpan.FromSeconds(2));
        for (int i = 0; i < 300; i++) congested.Capture(large, "127.0.0.1", 1, true);
        Check(congested.Dropped > 0, "Slow UI drops capture data without blocking packet processing");
        blocked.Release.TrySetResult(); await congested.DisposeAsync();
        Console.WriteLine($"PASS: {checks} desktop capture and injection checks.");
        return checks;
    }

    private sealed class BlockedWriter : StringWriter
    {
        internal TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public override async Task WriteLineAsync(string? value) { Entered.TrySetResult(); await Release.Task; }
    }
}
