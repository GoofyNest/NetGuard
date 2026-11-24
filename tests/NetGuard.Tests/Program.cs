using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Module.Helpers;
using Module.Networking;
using Module.PacketHandler;
using Module.PacketHandler.Filtering;
using Module.Services;
using SilkroadSecurityAPI;
using App = Module.Main;

var work = Path.Combine(Path.GetTempPath(), "NetGuardReviewTests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(work);
int passed = 0;
passed += await NetworkPerformanceTests.Run();
passed += await ReliabilityTests.Run(work);
void Check(bool ok, string message)
{
    if (!ok) throw new Exception(message);
    passed++;
}
void ReloadData(string path)
{
    try { typeof(Reader).GetMethod("ReloadAll", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [path]); }
    catch (TargetInvocationException ex) { throw ex.InnerException!; }
}

if (args.Length > 0)
{
    ReloadData(args[0]);
    Check(App.Chars.TryGetValue(3621, out var monster) && monster.Lvl == 140
        && monster.PD == 5247 && monster.MD == 7958 && monster.ExpToGive == 30462,
        "Extended character text field positions");
    Check(App.Chars.TryGetValue(24455, out var camel) && camel.Lvl == 90, "Camel level");
    Console.WriteLine($"Loaded: {App.Items.Count} items, {App.Skills.Count} skills, {App.Chars.Count} characters.");
}

// Failed reload leaves the complete previous generation untouched.
var previous = App.Data;
File.WriteAllText(Path.Combine(work, "characterdata.txt"), "1\tbroken\n", Encoding.Unicode);
try { ReloadData(work); throw new Exception("Malformed file accepted"); }
catch (FormatException) { }
Check(ReferenceEquals(App.Data, previous), "Failed reload retained old data");

var limiter = new ByteRateLimiter(100_000, 200_000);
Check(limiter.TryConsume(190_000), "Burst allowance");
Check(!limiter.TryConsume(100_000), "Burst excess rejected");
Check(limiter.TryConsume(1), "Rejected request does not replenish allowance");

var packet = new Packet(0x7034); packet.WriteUInt8(0x0D); packet.WriteUInt8(99); packet.Lock();
FilterRegistry.SetLanding(FilterContext.AgentClient, new ReadingLanding());
var result = FilterRegistry.RunLanding(FilterContext.AgentClient, packet, new SessionData(), null!);
Check(result == null && packet.ReadUInt8() == 0x0D, "Landing reads independent packet view");
FilterRegistry.SetLanding(FilterContext.AgentClient, new ThrowingLanding());
Check(FilterRegistry.RunLanding(FilterContext.AgentClient, packet, new SessionData(), null!)?.ResultType
    == PacketResultType.Block, "Landing exception fails closed");
FilterRegistry.ClearLandings();

// Force backpressure on a loopback socket and verify every byte arrives.
var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
using var sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { SendBufferSize = 1024 };
var connect = sender.ConnectAsync((IPEndPoint)listener.LocalEndpoint);
using var receiver = await listener.AcceptSocketAsync(); await connect; listener.Stop();
var bytes = new byte[2 * 1024 * 1024]; new Random(123).NextBytes(bytes);
var received = new byte[bytes.Length];
using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(10));
var sending = SocketIO.SendAllAsync(sender, bytes, deadline.Token).AsTask();
int offset = 0;
while (offset < received.Length)
{
    int count = await receiver.ReceiveAsync(received.AsMemory(offset, Math.Min(4096, received.Length-offset)),
        SocketFlags.None, deadline.Token);
    if (count == 0) throw new Exception("Early EOF");
    offset += count;
}
await sending;
Check(bytes.SequenceEqual(received), "Complete send under backpressure");

// Exercise exception cleanup and buffer return without forwarding any real game traffic.
var front = new TcpListener(IPAddress.Loopback, 0); front.Start();
using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
var joining = clientSocket.ConnectAsync((IPEndPoint)front.LocalEndpoint);
var proxySocket = await front.AcceptSocketAsync(); await joining; front.Stop();
bool disconnected = false;
var connection = new Module.Networking.Module(987654, proxySocket,
    (int id, ref Socket socket, Module.Config.ModuleType type) => { disconnected = true; socket.Dispose(); },
    Module.Config.ModuleType.AgentModule);
FilterRegistry.SetLanding(FilterContext.AgentClient, new ThrowingLanding()); // separate fail-closed check above
connection.Stop();
Check(disconnected && !FilterRegistry.LiveSessions.ContainsKey(987654), "Connection cleanup unregisters session");
var returned = (int[])typeof(Module.Networking.Module).GetField("_returnedBuffers", BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(connection)!;
Check(returned.All(x=>x==1), "Unstarted receive buffers returned once");
connection.Stop();
Check(returned.All(x=>x==1), "Repeated disconnect does not double-return");
FilterRegistry.ClearLandings();

Console.WriteLine($"PASS: {passed} core checks. Artifacts: {work}");

// Exercise the real proxy receive/forward path with two independent security
// handshakes and no opcode filters. A handshake must never reach Security.Send.
var handshakeFront = new TcpListener(IPAddress.Loopback, 0); handshakeFront.Start();
using var gameClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
var frontConnect = gameClientSocket.ConnectAsync((IPEndPoint)handshakeFront.LocalEndpoint);
var frontProxySocket = await handshakeFront.AcceptSocketAsync(); await frontConnect; handshakeFront.Stop();
var handshakeBack = new TcpListener(IPAddress.Loopback, 0); handshakeBack.Start();
var handshakeProxy = new Module.Networking.Module(987656, frontProxySocket,
    (int id, ref Socket socket, Module.Config.ModuleType type) => socket.Dispose(), Module.Config.ModuleType.AgentModule);
var moduleType = typeof(Module.Networking.Module);
object Private(string name) => moduleType.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(handshakeProxy)!;
var backProxySocket = (Socket)Private("ModuleSocket");
var backConnect = backProxySocket.ConnectAsync((IPEndPoint)handshakeBack.LocalEndpoint);
using var gameServerSocket = await handshakeBack.AcceptSocketAsync(); await backConnect; handshakeBack.Stop();
var handshakeLocalSecurity = (Security)Private("_localSecurity");
var gameClientSecurity = new Security(); var gameServerSecurity = new Security();
var receiveMethod = moduleType.GetMethod("HandleReceivedData", BindingFlags.NonPublic | BindingFlags.Instance)!;
async Task Inject(Security peer, bool fromClient)
{
    var frames = peer.TransferOutgoing();
    if (frames == null) return;
    foreach (var frame in frames)
    {
        var buffer = (byte[])Private(fromClient ? "_localBuffer" : "_remoteBuffer");
        frame.Key.Buffer.AsSpan(frame.Key.Offset, frame.Key.Size).CopyTo(buffer);
        await ((Task)receiveMethod.Invoke(handshakeProxy, [frame.Key.Size, fromClient])!).WaitAsync(TimeSpan.FromSeconds(5));
    }
}
async Task<Packet> ReceivePeer(Socket socket, Security peer, ushort wanted)
{
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    var buffer = new byte[8192];
    while (true)
    {
        int length = await socket.ReceiveAsync(buffer, SocketFlags.None, timeout.Token);
        if (length == 0) throw new Exception("Proxy closed during handshake regression");
        peer.Recv(buffer, 0, length);
        foreach (var incoming in peer.TransferIncoming()) if (incoming.Opcode == wanted) return incoming;
    }
}
try
{
    handshakeLocalSecurity.GenerateSecurity(true, true, true);
    foreach (var frame in handshakeLocalSecurity.TransferOutgoing()!)
        gameClientSecurity.Recv(frame.Key.Buffer, frame.Key.Offset, frame.Key.Size);
    gameClientSecurity.TransferIncoming();
    await Inject(gameClientSecurity, true);
    await ReceivePeer(gameClientSocket, gameClientSecurity, 0x5000);
    await Inject(gameClientSecurity, true);
    Check(true, "Client handshake completes without an opcode filter");

    // Also exercise a filter/landing that returns a plain default result.
    FilterRegistry.SetLanding(FilterContext.AgentServer, new PassingLanding());
    gameServerSecurity.GenerateSecurity(true, true, true);
    await Inject(gameServerSecurity, false);
    await ReceivePeer(gameServerSocket, gameServerSecurity, 0x5000);
    await Inject(gameServerSecurity, false);
    await ReceivePeer(gameServerSocket, gameServerSecurity, 0x9000);
    Check(true, "Server handshake completes with a default forwarding result");
    FilterRegistry.ClearLandings();

    var request = new Packet(0x7777, true); request.WriteUInt32(0x12345678); gameClientSecurity.Send(request);
    await Inject(gameClientSecurity, true);
    Check((await ReceivePeer(gameServerSocket, gameServerSecurity, 0x7777)).ReadUInt32() == 0x12345678,
        "Encrypted client packet forwarded after both handshakes");
    var reply = new Packet(0x7778, true); reply.WriteUInt32(0x87654321); gameServerSecurity.Send(reply);
    await Inject(gameServerSecurity, false);
    Check((await ReceivePeer(gameClientSocket, gameClientSecurity, 0x7778)).ReadUInt32() == 0x87654321,
        "Encrypted server packet forwarded after both handshakes");

    FilterRegistry.SetLanding(FilterContext.AgentClient, new RoutingLanding());
    for (uint i = 1; i <= 3; i++)
    {
        var mixed = new Packet(i == 2 ? (ushort)0x7779 : (ushort)0x7777, true);
        mixed.WriteUInt32(i); gameClientSecurity.Send(mixed);
    }
    var combined = gameClientSecurity.TransferOutgoing().SelectMany(f => f.Key.Buffer.AsSpan(f.Key.Offset, f.Key.Size).ToArray()).ToArray();
    combined.CopyTo((byte[])Private("_localBuffer"), 0);
    await (Task)receiveMethod.Invoke(handshakeProxy, [combined.Length, true])!;
    var ordered = new List<Packet>();
    using (var orderDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
    {
        byte[] readBuffer = new byte[8192];
        while (ordered.Count < 3)
        {
            int length = await gameServerSocket.ReceiveAsync(readBuffer, SocketFlags.None, orderDeadline.Token);
            if (length == 0) throw new Exception("Unexpected EOF in routing test");
            gameServerSecurity.Recv(readBuffer, 0, length);
            ordered.AddRange(gameServerSecurity.TransferIncoming());
        }
    }
    Check(ordered.Select(p => p.Opcode).SequenceEqual(new ushort[] { 0x7777, 0x7780, 0x7777 })
        && ordered.Select(p => p.ReadUInt32()).SequenceEqual(new uint[] { 1, 2, 3 }),
        "Ordinary and replacement packets retain upstream order in the same receive batch");
    Check((await ReceivePeer(gameClientSocket, gameClientSecurity, 0x7781)).ReadUInt32() == 2,
        "Explicit LocalSecurity replacement goes to client independently of SendImmediately");
    FilterRegistry.ClearLandings();

    frontProxySocket.SendBufferSize = 1024;
    gameClientSocket.ReceiveBufferSize = 1024;
    for (int i = 0; i < 128; i++)
    {
        var flood = new Packet(0x7778, true); flood.WriteUInt8Array(new byte[7000]);
        gameServerSecurity.Send(flood);
    }
    // Intentionally stop reading the client socket. This must not hold the
    // processing gate or prevent a client request from reaching the server.
    await Inject(gameServerSecurity, false);
    var whileSlow = new Packet(0x7777, true); whileSlow.WriteUInt32(999); gameClientSecurity.Send(whileSlow);
    await Inject(gameClientSecurity, true);
    Check((await ReceivePeer(gameServerSocket, gameServerSecurity, 0x7777)).ReadUInt32() == 999,
        "Slow client does not block opposite-direction packet processing");
}
finally { FilterRegistry.ClearLandings(); handshakeProxy.Stop(); }


// Compile the real hot filters twice and verify migration across assembly generations.
if (args.Length > 1)
{
    passed += await LoginDispatchTests.Run(args[1], work);
    string source = Path.Combine(args[1], "Module", "filters");
    string staged = Path.Combine(work, "filters");
    foreach (var file in Directory.GetFiles(source, "*.cs", SearchOption.AllDirectories))
    {
        string target = Path.Combine(staged, Path.GetRelativePath(source, file));
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        File.Copy(file, target);
    }
    using var loader = new FilterLoader(staged);
    var reload = typeof(FilterLoader).GetMethod("Reload", BindingFlags.Instance | BindingFlags.NonPublic)!;
    reload.Invoke(loader, null);
    Check(loader.Resolve(FilterContext.AgentClient, 0x704C) != null, "Real hot filters compile");
    var session = new SessionData();
    loader.AttachSession(session);
    var host = session.Host!;
    object Get(object o, string name) => o.GetType().GetProperty(name)!.GetValue(o)!;
    void Set(object o, string name, object value) => o.GetType().GetProperty(name)!.SetValue(o,value);
    var agent = Get(host,"Agent");
    var character = agent.GetType().GetMethod("EnsureCurrent")!.Invoke(agent,["migration-test"])!;
    Set(character,"UniqueID",123u);
    Set(character,"LastUseItem",(ushort)0x08CD);
    Set(character,"PetStateReady",true);
    Set(character,"AttackPetPending",true);
    var pending = (Queue<int>)Get(character,"PendingItemUses"); pending.Enqueue(1);
    var inventory = Get(character,"Inventory");
    var itemType = host.GetType().Assembly.GetType("NetGuard.Hot.ItemState")!;
    var item = Activator.CreateInstance(itemType)!;
    Set(item,"Id",999L); Set(item,"Slot",13);
    ((System.Collections.IList)Get(inventory,"Items")).Add(item);
    Set(inventory,"ItemCount",1);
    var petType = host.GetType().Assembly.GetType("NetGuard.Hot.CosState")!;
    var pet = Activator.CreateInstance(petType)!;
    Set(pet,"UniqueID",555u); Set(pet,"ObjectID",9330u); Set(pet,"Type",1); Set(pet,"Summoned",true);
    ((System.Collections.IList)Get(character,"Pets")).Add(pet);
    var cooldown = DateTime.UtcNow;
    Set(Get(host,"UseItem"),"AttackPetTime",cooldown);
    Set(Get(host,"Gateway"),"SentPatchResponse",true);
    var charBuffer = new Packet(0x3013); charBuffer.WriteUInt32(777);
    Set(agent,"CharData",charBuffer); Set(agent,"CharDataProcess",2);
    FilterRegistry.LiveSessions[42] = session;
    try
    {
        reload.Invoke(loader,null);
        Check(session.FilterStateValid && session.Host!.GetType() != host.GetType(), "New host generation migrated");
        var newAgent = Get(session.Host!,"Agent");
        var copied = ((System.Collections.IList)Get(newAgent,"Chars"))[0]!;
        Check((uint)Get(copied,"UniqueID")==123 && (ushort)Get(copied,"LastUseItem")==0x08CD, "Character scalar migration");
        Check(((System.Collections.IList)Get(Get(copied,"Inventory"),"Items")).Count==1, "Inventory entries migrated");
        Check(((System.Collections.IList)Get(copied,"Pets")).Count==1
            && ((Queue<int>)Get(copied,"PendingItemUses")).Peek()==1
            && (bool)Get(copied,"AttackPetPending"), "Pet guards migrated");
        Check((DateTime)Get(Get(session.Host!,"UseItem"),"AttackPetTime")==cooldown, "Cooldown preserved");
        Check((bool)Get(Get(session.Host!,"Gateway"),"SentPatchResponse"), "Gateway state preserved");
        var buffer = (Packet)Get(newAgent,"CharData"); buffer.Lock();
        Check(buffer.ReadUInt32()==777 && (int)Get(newAgent,"CharDataProcess")==2, "In-progress character buffer preserved");
    }
    finally { FilterRegistry.LiveSessions.TryRemove(42,out _); FilterRegistry.ClearLandings(); }
}


 // A malformed client frame must end the receive task and return its buffer.
var badListener = new TcpListener(IPAddress.Loopback,0); badListener.Start();
using var badClient = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
var badConnect = badClient.ConnectAsync((IPEndPoint)badListener.LocalEndpoint);
var badSocket = await badListener.AcceptSocketAsync(); await badConnect; badListener.Stop();
int closed = 0;
var badConnection = new Module.Networking.Module(987655,badSocket,
    (int id, ref Socket socket, Module.Config.ModuleType type) => { Interlocked.Exchange(ref closed,1); socket.Dispose(); },
    Module.Config.ModuleType.AgentModule);
var flags = BindingFlags.Instance|BindingFlags.NonPublic;
var localSecurity = (Security)typeof(Module.Networking.Module).GetField("_localSecurity",flags)!.GetValue(badConnection)!;
localSecurity.GenerateSecurity(true,true,true);
await (Task)typeof(Module.Networking.Module).GetMethod("DoReceive",flags)!.Invoke(badConnection,[true])!;
await SocketIO.SendAllAsync(badClient,new byte[]{0,0,0x34,0x12,0,0});
Check(SpinWait.SpinUntil(()=>Volatile.Read(ref closed)==1,3000), "Malformed frame disconnects rather than orphaning receive");
var badReturned = (int[])typeof(Module.Networking.Module).GetField("_returnedBuffers",flags)!.GetValue(badConnection)!;
Check(SpinWait.SpinUntil(()=>badReturned.All(x=>x==1),3000), "In-flight receive buffers returned after completion");

// Concurrent bus senders share one ordered socket writer.
var portProbe = new TcpListener(IPAddress.Loopback,0); portProbe.Start();
int busPort = ((IPEndPoint)portProbe.LocalEndpoint).Port; portProbe.Stop();
SessionBus.ApiPort=busPort; SessionBus.ListenAddress="127.0.0.1"; SessionBus.ApiPassword=new string('A',64);
var busSession = new SessionData(); SessionBus.Register(busSession);
int delivered=0;
BusRegistry.Dispatcher.On("test-message",(_,_)=>Interlocked.Increment(ref delivered));
SessionBus.StartApiListener();
try
{
    var sendMethod=typeof(SessionBus).GetMethod("SendToAsync",BindingFlags.Static|BindingFlags.NonPublic)!;
    var sends = Enumerable.Range(0,24).Select(i=>(Task)sendMethod.Invoke(null,
        new object[]{"127.0.0.1",busPort,SessionBus.ApiPassword,new BusMessage("test-message", new { Index=i })})!).ToArray();
    await Task.WhenAll(sends).WaitAsync(TimeSpan.FromSeconds(10));
    Check(SpinWait.SpinUntil(()=>Volatile.Read(ref delivered)==24,5000), "Concurrent bus frames delivered intact");
    using var unauthorized = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
    await unauthorized.ConnectAsync(IPAddress.Loopback,busPort);
    await SocketIO.SendAllAsync(unauthorized,new byte[]{1,0,(byte)'x',0,0,32,0}); // 2 MB > limit
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
    Check(await unauthorized.ReceiveAsync(new byte[1],SocketFlags.None,timeout.Token)==0, "Oversized unauthenticated frame rejected");
}
finally { SessionBus.Unregister(busSession); SessionBus.Stop(); }
// File drain and shutdown must complete even while the console writer is blocked.
var originalOutput = Console.Out;
var blocker = new BlockingWriter();
App.Config.LogFile = Path.Combine(work,"logging.txt");
App.Settings.DisableLogWriting = false;
App.Settings.DisableConsoleLogging = false;
Console.SetOut(blocker);
try
{
    Custom.StartDrain();
    Custom.StartDrain();
    Custom.WriteLine("blocked-output");
    Check(blocker.Entered.Wait(3000), "Console writer reached");
    Check(!blocker.OnThreadPool, "Console has a dedicated thread");
    Custom.WriteLine("file-continues-during-selection");
    string ReadLog()
    {
        if (!File.Exists(App.Config.LogFile)) return "";
        using var stream = new FileStream(App.Config.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
    Check(SpinWait.SpinUntil(()=>ReadLog().Contains("file-continues-during-selection"),5000),
        "Idle flush continues during selection");
    Custom.WriteLine("shutdown-tail");
    await Custom.FlushAndStopAsync().WaitAsync(TimeSpan.FromSeconds(5));
    Check(ReadLog().Contains("shutdown-tail"), "Shutdown drains the file queue before closing");
}
finally
{
    Console.SetOut(TextWriter.Null);
    blocker.Released.Set();
}
originalOutput.WriteLine($"PASS: {passed} core, migration and logging checks. Artifacts: {work}");
sealed class ReadingLanding : ILanding
{
    public PacketHandlingResult? Handle(Packet packet, SessionData client, SessionContext ctx)
    { packet.ReadUInt8(); return null; }
}
sealed class ThrowingLanding : ILanding
{
    public PacketHandlingResult? Handle(Packet packet, SessionData client, SessionContext ctx)
        => throw new InvalidOperationException("Expected test exception");
}

sealed class PassingLanding : ILanding
{
    public PacketHandlingResult? Handle(Packet packet, SessionData client, SessionContext ctx) => new();
}

sealed class RoutingLanding : ILanding
{
    public PacketHandlingResult? Handle(Packet packet, SessionData client, SessionContext ctx)
    {
        if (packet.Opcode != 0x7779) return null;
        var result = new PacketHandlingResult();
        uint value = packet.ReadUInt32();
        var upstream = new Packet(0x7780, true); upstream.WriteUInt32(value);
        var downstream = new Packet(0x7781, true); downstream.WriteUInt32(value);
        result.ModifiedPackets.Add(new PacketList { Packet = upstream, SecurityType = SecurityType.RemoteSecurity, SendImmediately = false });
        result.ModifiedPackets.Add(new PacketList { Packet = downstream, SecurityType = SecurityType.LocalSecurity, SendImmediately = true });
        return result;
    }
}

sealed class BlockingWriter : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;
    public ManualResetEventSlim Entered { get; } = new(false);
    public ManualResetEventSlim Released { get; } = new(false);
    public bool OnThreadPool;
    public override void WriteLine(string? value)
    {
        OnThreadPool = Thread.CurrentThread.IsThreadPoolThread;
        Entered.Set();
        Released.Wait();
    }
}

