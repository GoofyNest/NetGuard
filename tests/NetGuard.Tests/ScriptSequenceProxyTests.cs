using System.Text.Json;
using NetGuard.DesktopProtocol;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Module.Networking;
using Module.PacketHandler;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;
using Proxy = Module.Networking.Module;

internal static class ScriptSequenceProxyTests
{
    internal static async Task<int> Run()
    {
        int checks = 0;
        void Check(bool ok, string name) { if (!ok) throw new Exception(name); checks++; }
        var front = new TcpListener(IPAddress.Loopback, 0); front.Start();
        using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var connect = client.ConnectAsync((IPEndPoint)front.LocalEndpoint);
        var localSocket = await front.AcceptSocketAsync(); await connect; front.Stop();
        var previousLoader = FilterRegistry.Loader; FilterRegistry.Loader = null;
        var oldDiagnostics = DesktopDiagnostics.Current;
        await using var desktop = new DesktopDiagnostics(1, new StringWriter());
        DesktopDiagnostics.Current = desktop;
        var proxy = new Proxy(987661, localSocket, (int id, ref Socket socket, Module.Config.ModuleType type) => socket.Dispose(), Module.Config.ModuleType.AgentModule);
        object Field(string name) => typeof(Proxy).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(proxy)!;
        var upstreamSocket = (Socket)Field("ModuleSocket");
        var back = new TcpListener(IPAddress.Loopback, 0); back.Start();
        var connecting = upstreamSocket.ConnectAsync((IPEndPoint)back.LocalEndpoint);
        using var server = await back.AcceptSocketAsync(); await connecting; back.Stop();
        var clientSecurity = new Security(); var serverSecurity = new Security();
        var localSecurity = (Security)Field("_localSecurity"); var remoteSecurity = (Security)Field("_remoteSecurity");
        void Pump(Security source, Security target)
        {
            foreach (var frame in source.TransferOutgoing()) target.Recv(frame.Key.Buffer, frame.Key.Offset, frame.Key.Size);
            target.TransferIncoming();
        }
        void Handshake(Security host, Security peer)
        {
            host.GenerateSecurity(true, true, true);
            Pump(host, peer); Pump(peer, host); Pump(host, peer); Pump(peer, host);
        }
        Packet Packet(ushort opcode, byte value = 0)
        { var packet = new Packet(opcode, true); packet.WriteUInt8(value); return packet; }
        async Task Inject(bool fromClient, params Packet[] packets)
        {
            var source = fromClient ? clientSecurity : serverSecurity;
            foreach (var packet in packets) source.Send(packet);
            var bytes = source.TransferOutgoing().SelectMany(p => p.Key.Buffer.AsSpan(p.Key.Offset, p.Key.Size).ToArray()).ToArray();
            bytes.CopyTo((byte[])Field(fromClient ? "_localBuffer" : "_remoteBuffer"), 0);
            await ((Task)typeof(Proxy).GetMethod("HandleReceivedData", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(proxy, new object[] { bytes.Length, fromClient })!).WaitAsync(TimeSpan.FromSeconds(3));
        }
        var serverPackets = new Queue<Packet>(); var clientPackets = new Queue<Packet>();
        async Task<Packet> Read(bool atServer)
        {
            var queue = atServer ? serverPackets : clientPackets;
            var socket = atServer ? server : client;
            var security = atServer ? serverSecurity : clientSecurity;
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            while (queue.Count == 0)
            {
                var buffer = new byte[8192];
                int length = await socket.ReceiveAsync(buffer, SocketFlags.None, deadline.Token);
                if (length == 0) throw new Exception("Unexpected EOF");
                security.Recv(buffer, 0, length);
                foreach (var packet in security.TransferIncoming()) queue.Enqueue(packet);
            }
            return queue.Dequeue();
        }
        async Task Paused(string name)
        {
            Check(serverPackets.Count == 0, name + " (no buffered next step)");
            using var deadline = new CancellationTokenSource(50);
            try { await server.ReceiveAsync(new byte[8192], SocketFlags.None, deadline.Token); throw new Exception(name); }
            catch (OperationCanceledException) when (deadline.IsCancellationRequested) { Check(true, name); }
        }
        try
        {
            await desktop.HandleCommandAsync(JsonSerializer.Serialize(new DesktopCommand("inject",
                Injection: new(proxy.ClientId, true, 0x77E1, [42], true, false)), Protocol.Json));
            Check(server.Available == 0, "Desktop injection cannot bypass the handshake");
            Handshake(localSecurity, clientSecurity); Handshake(serverSecurity, remoteSecurity);
            await desktop.HandleCommandAsync(JsonSerializer.Serialize(new DesktopCommand("inject",
                Injection: new(proxy.ClientId, true, 0x77E1, [42], true, false)), Protocol.Json));
            var remoteInjection = await Read(true);
            Check(remoteInjection.Opcode == 0x77E1 && remoteInjection.ReadUInt8() == 42, "Desktop Remote injection reaches the real server socket");
            await desktop.HandleCommandAsync(JsonSerializer.Serialize(new DesktopCommand("inject",
                Injection: new(proxy.ClientId, false, 0x77E2, [43], true, false)), Protocol.Json));
            var localInjection = await Read(false);
            Check(localInjection.Opcode == 0x77E2 && localInjection.ReadUInt8() == 43, "Desktop Local injection reaches the real client socket");
            FilterRegistry.SetLanding(FilterContext.AgentClient, new SequenceLanding());
            await Inject(true, Packet(0x7770));
            Check((await Read(true)).Opcode == 0x7771, "ModifiedPackets starts scripted sequence instead of forwarding trigger");
            await Paused("Proxy releases processing gate while sequence waits");
            await Inject(true, Packet(0x7788));
            Check((await Read(true)).Opcode == 0x7788, "Ordinary client traffic forwards during wait");
            await Inject(true, Packet(0xB771));
            Check((await Read(true)).Opcode == 0xB771, "Client packet matching expected server opcode still forwards normally");
            await Paused("Client-origin opcode does not satisfy server wait");
            await Inject(false, Packet(0xB099, 99));
            var unrelated = await Read(false);
            Check(unrelated.Opcode == 0xB099 && unrelated.ReadUInt8() == 99, "Unrelated server packet forwards intact during wait");
            await Paused("Unrelated server opcode does not advance sequence");

            // Both replies existed before the second instruction could be sent.
            // Only the first may satisfy the currently registered wait.
            await Inject(false, Packet(0xB771, 0), Packet(0xB771, 2));
            Check((await Read(false)).ReadUInt8() == 0 && (await Read(false)).ReadUInt8() == 2,
                "Reply payloads are forwarded intact and not interpreted as success/failure");
            Check((await Read(true)).Opcode == 0x7772, "Server feedback releases next scripted packet");
            await Paused("Old reply in same receive batch cannot release the next step");
            await Inject(false, Packet(0xB771));
            Check((await Read(true)).Opcode == 0x7773, "Second reply releases final instruction");
            await Read(false);
            await Inject(true, Packet(0x7770));
            Check((await Read(true)).Opcode == 0x7771, "Another sequence can start");
            proxy.Stop();
            await ((ScriptedPacketSequence)Field("_scriptedPackets")).Completion.WaitAsync(TimeSpan.FromSeconds(3));
            Check(true, "Proxy disconnect cancels active scripting wait");
        }
        finally { proxy.Stop(); DesktopDiagnostics.Current = oldDiagnostics; FilterRegistry.ClearLandings(); FilterRegistry.Loader = previousLoader; }
        Console.WriteLine($"PASS: {checks} proxy sequencing checks.");
        return checks;
    }

    private sealed class SequenceLanding : ILanding
    {
        public PacketHandlingResult? Handle(Packet packet, SessionData session, SessionContext context)
        {
            if (packet.Opcode != 0x7770) return null;
            var result = new PacketHandlingResult();
            result.ModifiedPackets.Add(new PacketList { Packet = new Packet(0x7771), SecurityType = SecurityType.RemoteSecurity, ServerReply = 0xB771 });
            result.ModifiedPackets.Add(new PacketList { Packet = new Packet(0x7772), SecurityType = SecurityType.RemoteSecurity, ServerReply = 0xB771 });
            result.ModifiedPackets.Add(new PacketList { Packet = new Packet(0x7773), SecurityType = SecurityType.RemoteSecurity });
            return result;
        }
    }
}
