using System.Threading.Channels;
using Module.Networking;
using Module.PacketHandler;
using SilkroadSecurityAPI;

internal static class ScriptedPacketSequenceTests
{
    internal static async Task<int> Run()
    {
        int checks = 0;
        void Check(bool value, string name) { if (!value) throw new Exception(name); checks++; }
        var sent = Channel.CreateUnbounded<ushort>();
        var logs = Channel.CreateUnbounded<string>();
        using var gate = new SemaphoreSlim(1);
        var runner = new ScriptedPacketSequence(async (packet, toServer, register, token) =>
        {
            await gate.WaitAsync(token);
            try { register(); sent.Writer.TryWrite(packet.Opcode); }
            finally { gate.Release(); }
        }, text => logs.Writer.TryWrite(text));
        PacketList Step(ushort opcode, ushort? reply = null, int timeout = 2000) => new()
        { Packet = new Packet(opcode), SecurityType = SecurityType.RemoteSecurity, ServerReply = reply, ServerReplyTimeoutMs = timeout };
        async Task<ushort> Next() => await sent.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(3));
        async Task NoSend(string name)
        {
            using var deadline = new CancellationTokenSource(40);
            try { await sent.Reader.ReadAsync(deadline.Token); throw new Exception(name); }
            catch (OperationCanceledException) when (deadline.IsCancellationRequested) { Check(true, name); }
        }
        try
        {
            runner.Enqueue(new[] { Step(1, 0xB001), Step(2, 0xB002), Step(3) }, true);
            Check(await Next() == 1, "First step sent without waiting for a response first");
            await NoSend("Next step waits");
            runner.ObserveServerPacket(0xB099);
            await NoSend("Unrelated opcode cannot release wait");
            runner.Enqueue(new[] { Step(4, 0xB004), Step(5) }, true);
            runner.ObserveServerPacket(0xB001);
            Check(await Next() == 2, "Matching response advances exactly one step");
            await NoSend("Second sequence does not interleave with first");
            runner.ObserveServerPacket(0xB002);
            Check(await Next() == 3 && await Next() == 4, "Sequences preserve FIFO order");
            runner.ObserveServerPacket(0xB004);
            Check(await Next() == 5, "Last waiting sequence completes");

            runner.Enqueue(new[] { Step(6, 0xB006, 40), Step(7) }, true);
            Check(await Next() == 6, "Timeout sequence starts");
            var log = await logs.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(3));
            Check(log.Contains("0xB006") && log.Contains("cancelled"), "Timeout logs expected opcode and cancels tail");
            runner.ObserveServerPacket(0xB006);
            await NoSend("Late reply cannot revive timed-out sequence");
            runner.Enqueue(new[] { Step(8, 0xB008) }, true);
            Check(await Next() == 8, "New sequence runs after previous timeout");
            runner.Stop();
            await runner.Completion.WaitAsync(TimeSpan.FromSeconds(3));
            runner.ObserveServerPacket(0xB008);
            runner.Enqueue(new[] { Step(9) }, true);
            await NoSend("Disconnect cancels waits and rejects later instructions");
        }
        finally { runner.Stop(); await runner.Completion; }

        // A response can arrive immediately after the write: registration must
        // happen first, even before the send delegate returns.
        ScriptedPacketSequence? immediate = null;
        var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        immediate = new ScriptedPacketSequence((packet, _, register, _) =>
        {
            register();
            if (packet.Opcode == 10) immediate!.ObserveServerPacket(0xB010);
            if (packet.Opcode == 11) completed.TrySetResult();
            return Task.CompletedTask;
        }, _ => { });
        try
        {
            immediate.Enqueue(new[] { Step(10, 0xB010), Step(11) }, true);
            await completed.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Check(true, "Immediate server reply is not lost");
        }
        finally { immediate.Stop(); await immediate.Completion; }
        var rejected = new List<string>();
        var bounded = new ScriptedPacketSequence((_, _, _, _) => throw new Exception("Oversized sequence was sent"), rejected.Add);
        try
        {
            bounded.Enqueue(new[] { new PacketList { Packet = new Packet(1, false, true, new byte[8 * 1024 * 1024]), ServerReply = 2 } }, true);
            Check(rejected.Count == 1 && rejected[0].Contains("byte limit"), "Script byte limit rejects before copying oversized payload");
        }
        finally { bounded.Stop(); await bounded.Completion; }
        Console.WriteLine($"PASS: {checks} scripted sequence checks.");
        return checks;
    }
}
