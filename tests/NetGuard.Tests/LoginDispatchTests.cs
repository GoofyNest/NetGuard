using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Module.Networking;
using Module.PacketHandler;
using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;
using App = Module.Main;
using ProxyConnection = Module.Networking.Module;

internal static class LoginDispatchTests
{
    public static async Task<int> Run(string repository, string work)
    {
        int checks = 0;
        void Check(bool condition, string name) { if (!condition) throw new Exception(name); checks++; }
        string source = Path.Combine(repository, "build", "Debug", "filters");
        string staged = Path.Combine(work, "login-filters");
        foreach (var file in Directory.GetFiles(source, "*.cs", SearchOption.AllDirectories))
        {
            string target = Path.Combine(staged, Path.GetRelativePath(source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!); File.Copy(file, target);
        }
        using var loader = new FilterLoader(staged);
        typeof(FilterLoader).GetMethod("Reload", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(loader, null);
        var previousLoader = FilterRegistry.Loader; FilterRegistry.Loader = loader;
        string oldIp = App.Module.ModuleIP; int oldPort = App.Module.ModulePort;
        ProxyConnection? proxy = null;
        try
        {
            var session = new SessionData(); loader.AttachSession(session);
            var otherSession = new SessionData(); loader.AttachSession(otherSession);
            session.PlayerInfo.Username = "first"; otherSession.PlayerInfo.Username = "second";
            var targeted = new BusMessage("gateway.login_requested", System.Text.Json.JsonSerializer.SerializeToElement(
                new { Username = "first", Target = $"{App.Module.ModuleIP}:{App.Module.ModulePort}" }));
            BusRegistry.Dispatcher.Dispatch(session, targeted);
            BusRegistry.Dispatcher.Dispatch(otherSession, targeted);
            Check(session.Agent.LoginRequested && !otherSession.Agent.LoginRequested, "Login bus notification only updates matching user");
            session.Agent.LoginRequested = false;
            BusRegistry.Dispatcher.Dispatch(session, new BusMessage("gateway.login_requested", System.Text.Json.JsonSerializer.SerializeToElement(
                new { Username = "first", Target = "wrong-host:0" })));
            Check(!session.Agent.LoginRequested, "Login bus notification for other destination ignored");
            session.PlayerInfo.Username = "";
            BusRegistry.Dispatcher.Dispatch(session, targeted);
            Check(!session.Agent.LoginRequested, "Unauthenticated unknown session is not matched by broadcast");
            Packet Make(ushort opcode, Action<Packet>? write = null)
            { var packet = new Packet(opcode); write?.Invoke(packet); packet.Lock(); return packet; }
            var handshake = Make(0x5000);
            var actualLanding = FilterRegistry.RunLanding(FilterContext.AgentClient, handshake, session, null!);
            Check(actualLanding is { ResultType: PacketResultType.None }, "Real Agent landing returns an allow result");
            var handler = Module.PacketHandler.Agent.Client.PacketHandler.GetHandler(handshake, session, null!);
            Check(handler.Handle(handshake, session, null!).ResultType == PacketResultType.SkipSending,
                "Real handshake filter runs after real Agent landing");
            var identity = Make(0x2001, p => { p.WriteAscii("SR_Client"); p.WriteUInt8(0); });
            Check(Module.PacketHandler.Agent.Client.PacketHandler.GetHandler(identity, session, null!)
                .Handle(identity, session, null!).ResultType == PacketResultType.DoReceive,
                "Real identification filter starts upstream receiving");
            Check(FilterRegistry.StopsDispatch(new PacketHandlingResult { ResultType = PacketResultType.Block }), "Blocks remain terminal");
            Check(FilterRegistry.StopsDispatch(new PacketHandlingResult { ResultType = PacketResultType.Disconnect }), "Disconnects remain terminal");
            var replacement = new PacketHandlingResult(); replacement.ModifiedPackets.Add(new PacketList { Packet = Make(1) });
            Check(FilterRegistry.StopsDispatch(replacement), "Landing replacements remain terminal");

            IPacketHandler Dispatch(FilterContext context, Packet packet) => context switch
            {
                FilterContext.AgentClient => Module.PacketHandler.Agent.Client.PacketHandler.GetHandler(packet, session, null!),
                FilterContext.AgentServer => Module.PacketHandler.Agent.Server.PacketHandler.GetHandler(packet, session, null!),
                FilterContext.GatewayClient => Module.PacketHandler.Gateway.Client.PacketHandler.GetHandler(packet, session, null!),
                _ => Module.PacketHandler.Gateway.Server.PacketHandler.GetHandler(packet, session, null!)
            };
            foreach (FilterContext context in Enum.GetValues<FilterContext>())
            {
                FilterRegistry.SetLanding(context, new FixedLanding(new PacketHandlingResult()));
                Check(Dispatch(context, handshake).Handle(handshake, session, null!).ResultType == PacketResultType.SkipSending,
                    $"{context} default landing permits handshake filter");
                FilterRegistry.SetLanding(context, new FixedLanding(new PacketHandlingResult { ResultType = PacketResultType.Block }));
                Check(Dispatch(context, handshake).Handle(handshake, session, null!).ResultType == PacketResultType.Block,
                    $"{context} explicit block still wins");
                FilterRegistry.SetLanding(context, new FixedLanding(replacement));
                Check(ReferenceEquals(Dispatch(context, handshake).Handle(handshake, session, null!), replacement),
                    $"{context} replacement still wins");
            }
            // Restore the actual Landing classes for the socket-level startup test.
            typeof(FilterLoader).GetMethod("Reload", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(loader, null);
            // This synthetic session is not registered in LiveSessions.
            loader.AttachSession(session);

            session.Agent.IsIngame = true;
            bool IsTeleporting()
            {
                var agent = session.Host!.GetType().GetProperty("Agent")!.GetValue(session.Host)!;
                return (bool)agent.GetType().GetProperty("IsTeleporting")!.GetValue(agent)!;
            }
            foreach (ushort endOpcode in new ushort[] { 0x3012, 0x3077 })
            {
                Dispatch(FilterContext.AgentServer, Make(0x34B5));
                Check(IsTeleporting(), "Server reset starts teleport lock");
                foreach (ushort blocked in new ushort[] { 0x7074, 0x705A, 0x7034, 0xFFFF })
                {
                    var request = Make(blocked);
                    Check(Dispatch(FilterContext.AgentClient, request).Handle(request, session, null!).ResultType == PacketResultType.Block,
                        $"Teleport blocks client opcode 0x{blocked:X4} before parsing");
                }
                foreach (ushort allowed in new ushort[] { 0x34B6, 0x3012, 0x3014, 0x2002 })
                {
                    var request = Make(allowed);
                    Check(Dispatch(FilterContext.AgentClient, request).Handle(request, session, null!).ResultType != PacketResultType.Block,
                        $"Teleport permits required client opcode 0x{allowed:X4}");
                    Check(IsTeleporting(), "Client ready cannot clear teleport lock");
                }
                Dispatch(FilterContext.AgentServer, Make(endOpcode));
                Check(!IsTeleporting(), $"Server 0x{endOpcode:X4} ends teleport lock");
                var unknown = Make(0xFFFF);
                Check(Dispatch(FilterContext.AgentClient, unknown).Handle(unknown, session, null!).ResultType == PacketResultType.None,
                    "Normal dispatch resumes after teleport");
            }
            session.Agent.IsIngame = false;

            var front = new TcpListener(IPAddress.Loopback, 0); front.Start();
            using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var connecting = client.ConnectAsync((IPEndPoint)front.LocalEndpoint);
            var accepted = await front.AcceptSocketAsync(); await connecting; front.Stop();
            var backend = new TcpListener(IPAddress.Loopback, 0); backend.Start();
            App.Module.ModuleIP = "127.0.0.1"; App.Module.ModulePort = ((IPEndPoint)backend.LocalEndpoint).Port;
            proxy = new ProxyConnection(987657, accepted,
                (int id, ref Socket socket, Module.Config.ModuleType kind) => socket.Dispose(), Module.Config.ModuleType.AgentModule);
            var starting = proxy.StartAsync();
            using var server = await backend.AcceptSocketAsync(); backend.Stop(); await starting;
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            async Task Flush(Socket socket, Security security)
            {
                var frames = security.TransferOutgoing(); if (frames == null) return;
                foreach (var frame in frames)
                    await SocketIO.SendAllAsync(socket, frame.Key.Buffer.AsMemory(frame.Key.Offset, frame.Key.Size), deadline.Token);
            }
            async Task<Packet> ReceiveUntil(Socket socket, Security security, ushort opcode)
            {
                var buffer = new byte[8192];
                while (true)
                {
                    int length = await socket.ReceiveAsync(buffer, SocketFlags.None, deadline.Token);
                    if (length == 0) throw new Exception("Login proxy disconnected");
                    security.Recv(buffer, 0, length);
                    var incoming = security.TransferIncoming();
                    await Flush(socket, security);
                    foreach (var packet in incoming) if (packet.Opcode == opcode) return packet;
                }
            }
            var clientSecurity = new Security(); var serverSecurity = new Security();
            serverSecurity.GenerateSecurity(true, true, true);
            await Flush(server, serverSecurity);
            // No reflection or manual call to arm upstream receiving: 0x2001 must do it.
            var serverHandshake = ReceiveUntil(server, serverSecurity, 0x9000);
            await ReceiveUntil(client, clientSecurity, 0x5000);
            await ReceiveUntil(client, clientSecurity, 0x5000);
            await serverHandshake;
            Check(true, "Real StartAsync and identification complete both handshakes");
            Check(typeof(ProxyConnection).GetField("_handshakeTimer", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(proxy) == null,
                "Both accepted handshakes cancel handshake deadline");
            var login = new Packet(0x6103, true);
            login.WriteUInt32(123); login.WriteAscii("test-user"); login.WriteAscii("test-password");
            login.WriteUInt8(0); login.WriteUInt8Array(new byte[6]); clientSecurity.Send(login);
            await Flush(client, clientSecurity);
            var forwarded = await ReceiveUntil(server, serverSecurity, 0x6103);
            Check(forwarded.ReadUInt32() == 123 && forwarded.ReadAscii() == "test-user", "Login request reaches upstream intact");
            Check(FilterRegistry.LiveSessions[987657].PlayerInfo.Username == "test-user", "Authentication opcode handler executed");
            var reply = new Packet(0xA103, true); reply.WriteUInt8(1); serverSecurity.Send(reply);
            await Flush(server, serverSecurity);
            Check((await ReceiveUntil(client, clientSecurity, 0xA103)).ReadUInt8() == 1, "Successful login response reaches client");
            Check(typeof(ProxyConnection).GetField("_loginTimer", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(proxy) == null,
                "Successful login cancels login deadline");
            var upstreamSecurity = (Security)typeof(ProxyConnection).GetField("_remoteSecurity", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(proxy)!;
            upstreamSecurity.MassivePacketTimeout = TimeSpan.FromMilliseconds(50);
            var incomplete = new Packet(0x600D);
            incomplete.WriteUInt8(1); incomplete.WriteUInt16(1); incomplete.WriteUInt16(0x3013);
            serverSecurity.Send(incomplete); await Flush(server, serverSecurity);
            bool closedForAssembly = false;
            try { closedForAssembly = await client.ReceiveAsync(new byte[1], SocketFlags.None, deadline.Token) == 0; }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset) { closedForAssembly = true; }
            Check(closedForAssembly && upstreamSecurity.MassiveTimeRemaining <= TimeSpan.Zero,
                "Incomplete massive assembly closes idle socket without requiring another packet");
            return checks;
        }
        finally
        {
            proxy?.Stop(); App.Module.ModuleIP = oldIp; App.Module.ModulePort = oldPort;
            FilterRegistry.Loader = previousLoader; FilterRegistry.ClearLandings();
        }
    }
    private sealed class FixedLanding(PacketHandlingResult result) : ILanding
    {
        public PacketHandlingResult? Handle(Packet packet, SessionData session, SessionContext context) => result;
    }
}
