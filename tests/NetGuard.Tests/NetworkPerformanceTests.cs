using System.Net;
using System.Net.Sockets;
using Module.Networking;
using SilkroadSecurityAPI;

internal static class NetworkPerformanceTests
{
    internal static async Task<int> Run()
    {
        int checks = 0;
        void Check(bool value, string name) { if (!value) throw new Exception(name); checks++; }
        Packet Make(uint value, bool encrypted = true)
        {
            var packet = new Packet(0x7777, encrypted);
            packet.WriteUInt32(value);
            packet.Lock();
            return packet;
        }
        var queue = new BoundedPacketQueue();
        var mutable = new Packet(0x7777); mutable.WriteUInt8(1);
        Check(queue.TryEnqueue(mutable), "Context queue accepts packet");
        mutable.WriteUInt8(2);
        Check(queue.TryDequeue(out var frozen) && frozen.PayloadLength == 1 && frozen.ReadUInt8() == 1,
            "Context queue snapshots mutable packets");
        var oversized = new Packet(0x7777, false, true, new byte[BoundedPacketQueue.MaxBytes]);
        Check(!queue.TryEnqueue(oversized), "Context queue rejects oversized packet before cloning");
        for (int i = 0; i < 4096; i++) if (!queue.TryEnqueue(new Packet(1))) throw new Exception("Context queue prematurely full");
        Check(!queue.TryEnqueue(new Packet(1)), "Context queue bounds empty packets too");
        Check(queue.TryDequeue(out _) && queue.TryEnqueue(new Packet(1)), "Dequeue releases queue capacity");
        var source = Make(0x01020304);
        var copy = new Packet(source);
        Check(copy.ReadUInt8() == 4 && source.ReadUInt8() == 4, "Shared clones have independent cursors");
        source.GetBytes()[1] = 99;
        Check(copy.ReadUInt8() == 3 && source.ReadUInt8() == 99, "Exposing original detaches shared payload and preserves cursor");
        var second = new Packet(copy);
        copy.GetBytes()[0] = 88;
        Check(second.ReadUInt8() == 4, "Exposing clone cannot mutate its sibling");
        var escaped = source.GetBytes();
        var third = new Packet(source);
        escaped[0] = 77;
        Check(third.ReadUInt8() == 4, "Previously exposed arrays cannot mutate subsequent clones");

        // Fragment every handshake and encrypted frame, including its length prefix.
        foreach (bool encrypt in new[] { false, true })
        {
            var server = new Security(); var client = new Security();
            var observed = new List<Packet>();
            void Transfer(Security from, Security to, bool record = false)
            {
                foreach (var frame in from.TransferOutgoing())
                {
                    int end = frame.Key.Offset + frame.Key.Size;
                    for (int pos = frame.Key.Offset; pos < end;)
                    {
                        int length = Math.Min(pos % 17 + 1, end - pos);
                        to.Recv(frame.Key.Buffer, pos, length);
                        pos += length;
                    }
                    var packets = to.TransferIncoming();
                    if (record) observed.AddRange(packets);
                }
            }
            // A gameplay packet queued before handshake must follow acceptance
            // and the generated identification packet, never bypass them.
            client.Send(Make(123, encrypt));
            Check(client.TransferOutgoing().Count == 0, "Gameplay waits for handshake");
            server.GenerateSecurity(encrypt, true, true);
            Transfer(server, client); Transfer(client, server);
            Transfer(server, client); Transfer(client, server, true);
            Check(observed.Select(p => p.Opcode).SequenceEqual(new ushort[] { 0x9000, 0x2001, 0x7777 }),
                "Handshake priority and queued gameplay order are preserved: " + string.Join(",", observed.Select(p => p.Opcode.ToString("X4"))));
            observed.Clear();
            for (uint i = 0; i < 256; i++) client.Send(Make(i, encrypt));
            Transfer(client, server, true);
            Check(observed.Count == 256 && observed.Select(p => p.ReadUInt32()).SequenceEqual(Enumerable.Range(0, 256).Select(i => (uint)i)),
                "Fragmented encrypted burst preserves count/CRC and packet order");
            observed.Clear();
            byte[] payload = new byte[100_000]; new Random(42).NextBytes(payload);
            var massive = new Packet(0x3013, false, true); massive.WriteUInt8Array(payload);
            server.Send(massive); server.Send(Make(456, encrypt));
            Transfer(server, client, true);
            Check(observed.Count == 2 && observed[0].Opcode == 0x3013 && observed[0].Massive
                && observed[0].GetBytes().SequenceEqual(payload) && observed[1].ReadUInt32() == 456,
                "Massive fragments reconstruct exactly before following encrypted packet");
        }

        var saturated = new Security();
        for (uint i = 0; i < Security.MaxPendingSendPackets; i++) saturated.Send(Make(i));
        bool rejected = false;
        try { saturated.Send(Make(99)); } catch (SilkroadSecurityAPI.SecurityException) { rejected = true; }
        Check(rejected && saturated.PendingSendPackets == Security.MaxPendingSendPackets,
            "Security rejects packet-count overflow before handshake");
        Check(saturated.TransferOutgoing(17, 1024).Count == 0, "Full gameplay queue still waits for handshake");
        var handshakePeer = new Security(); handshakePeer.GenerateSecurity(true, true, true);
        var seen = new List<Packet>();
        for (int round = 0; round < 4; round++)
        {
            foreach (var (from, to) in new[] { (handshakePeer, saturated), (saturated, handshakePeer) })
            {
                while (true)
                {
                    var frames = from.TransferOutgoing(17, 1024);
                    if (frames.Count > 17 || frames.Sum(f => f.Key.Size) > 1024)
                        throw new Exception("Security transfer exceeds batch limits");
                    if (frames.Count == 0) break;
                    foreach (var frame in frames) to.Recv(frame.Key.Buffer, frame.Key.Offset, frame.Key.Size);
                    var incoming = to.TransferIncoming();
                    if (ReferenceEquals(to, handshakePeer)) seen.AddRange(incoming);
                }
            }
        }
        Check(seen.Where(p => p.Opcode == 0x7777).Select(p => p.ReadUInt32())
            .SequenceEqual(Enumerable.Range(0, Security.MaxPendingSendPackets).Select(i => (uint)i)),
            "Full queue drains in order across bounded batches after handshake");
        Check(saturated.PendingSendBytes == 0 && saturated.PendingSendPackets == 0,
            "Security queue accounting returns to zero");
        var byteLimited = new Security();
        rejected = false;
        try { byteLimited.Send(new Packet(0x7777, false, true, new byte[Security.MaxPendingSendBytes])); }
        catch (SilkroadSecurityAPI.SecurityException) { rejected = true; }
        Check(rejected && byteLimited.PendingSendBytes == 0, "Security rejects byte overflow without retaining payload");
        var mutableSend = new Packet(0x7777); mutableSend.WriteUInt8(7);
        saturated.Send(mutableSend); mutableSend.WriteUInt8(8);
        var captured = saturated.TransferOutgoing(1, 1024);
        Check(captured.Single().Value.PayloadLength == 1, "Security snapshots prevent caller mutation of queued packets");

        var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { SendBufferSize = 1024 };
        var connecting = socket.ConnectAsync((IPEndPoint)listener.LocalEndpoint);
        using var peer = await listener.AcceptSocketAsync(); await connecting; listener.Stop();
        Exception? failure = null;
        var sender = new OrderedSocketSender(socket, ex => failure = ex);
        try
        {
            byte[] expected = new byte[1024 * 1024]; new Random(11).NextBytes(expected);
            bool accepted = true;
            for (int i = 0; i < expected.Length; i += 1024)
                accepted &= sender.TryEnqueue(expected.AsMemory(i, 1024));
            Check(accepted, "Ordered sender accepts bounded burst");
            var flushed = sender.FlushAsync();
            var finishing = sender.FinishAsync();
            byte[] actual = new byte[expected.Length];
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            int position = 0;
            while (position < actual.Length)
            {
                int count = await peer.ReceiveAsync(actual.AsMemory(position), SocketFlags.None, deadline.Token);
                if (count == 0) throw new Exception("Unexpected EOF");
                position += count;
            }
            Check(failure == null && actual.SequenceEqual(expected), "Batched socket sender preserves every byte under backpressure");
            await flushed.WaitAsync(TimeSpan.FromSeconds(5));
            Check(flushed.IsCompletedSuccessfully, "Writer receipt completes after ordered bytes are written");
            await finishing.WaitAsync(TimeSpan.FromSeconds(5));
            Check(!sender.TryEnqueue(new byte[1]), "Graceful completion drains the tail and rejects new writes");
            sender.Stop();
            sender = new OrderedSocketSender(socket, ex => failure = ex);
            bool full = false;
            byte[] block = new byte[OrderedSocketSender.BatchBytes];
            for (int i = 0; i < 1024; i++)
                if (!sender.TryEnqueue(block)) { full = true; break; }
            Check(full, "Slow peer reaches bounded queue limit without silently dropping bytes");
            var blockedReceipt = sender.FlushAsync();
            bool drainExpired = false;
            try { await OrderedSocketSender.DrainAsync(new[] { sender }, TimeSpan.FromMilliseconds(50)); }
            catch (TimeoutException) { drainExpired = true; }
            Check(drainExpired, "Overall drain deadline aborts blocked sender");
            try { await blockedReceipt.WaitAsync(TimeSpan.FromSeconds(3)); throw new Exception("Stopped writer acknowledged unsent data"); }
            catch (Exception ex) when (ex is OperationCanceledException or IOException) { Check(true, "Aborted writer fails outstanding receipts"); }
        }
        finally { sender.Stop(); await sender.Completion.WaitAsync(TimeSpan.FromSeconds(5)); }
        Check(!sender.TryEnqueue(new byte[1]), "Stopped sender rejects new work");
        Console.WriteLine($"PASS: {checks} security and ordered-sender checks.");
        return checks;
    }
}
