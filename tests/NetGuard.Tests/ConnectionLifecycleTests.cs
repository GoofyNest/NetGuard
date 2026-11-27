using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Module.Config;
using Module.Networking;
using Module.PacketHandler.Filtering;
using App = Module.Main;

internal static class ConnectionLifecycleTests
{
    internal static async Task<int> Run()
    {
        int checks = 0;
        void Check(bool ok, string message) { if (!ok) throw new Exception(message); checks++; }
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var previousModule = App.Module;
        var previousLoader = FilterRegistry.Loader;
        var upstream = new TcpListener(IPAddress.Loopback, 0);
        upstream.Start();
        App.Module = new ModuleSettings { ModuleIP = "127.0.0.1", ModulePort = ((IPEndPoint)upstream.LocalEndpoint).Port };
        var clients = new List<Socket>();
        var servers = new List<Socket>();
        var proxy = new AsyncServer();
        try
        {
            FilterRegistry.Loader = null;
            var listening = proxy.StartAsync("127.0.0.1", 0, ModuleType.AgentModule);
            var listener = (Socket)typeof(AsyncServer).GetField("_listenerSocket", flags)!.GetValue(proxy)!;
            for (int i = 0; i < 4; i++)
            {
                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clients.Add(client);
                await client.ConnectAsync(listener.LocalEndPoint!);
                servers.Add(await upstream.AcceptSocketAsync().WaitAsync(TimeSpan.FromSeconds(3)));
            }
            var modules = (System.Collections.Concurrent.ConcurrentDictionary<int, Module.Networking.Module>)
                typeof(AsyncServer).GetField("_activeClients", flags)!.GetValue(proxy)!;
            var connections = modules.Values.ToArray();
            Check(connections.Length == 4, "Server tracks accepted connections");
            await proxy.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));
            await listening;
            Check(proxy.GetActiveConnections() == 0, "Async shutdown removes all accepted connections");
            foreach (var connection in connections)
            {
                var receives = (Task[])typeof(Module.Networking.Module).GetField("_receiveTasks", flags)!.GetValue(connection)!;
                var returned = (int[])typeof(Module.Networking.Module).GetField("_returnedBuffers", flags)!.GetValue(connection)!;
                Check(receives.All(t => t.IsCompleted) && returned.All(n => n == 1), "Shutdown awaits receives and returns pooled buffers");
            }
            await proxy.DisposeAsync();
            Check(true, "Repeated async shutdown is safe");

            // Simulate a hot host failing to attach during construction.
            using var failingLoader = new FilterLoader(Path.GetTempPath());
            typeof(FilterLoader).GetField("_attach", flags)!.SetValue(failingLoader,
                (Action<SessionData>)(_ => throw new InvalidOperationException("Injected attach failure")));
            FilterRegistry.Loader = failingLoader;
            var failedServer = new AsyncServer();
            try
            {
                var failing = failedServer.StartAsync("127.0.0.1", 0, ModuleType.AgentModule);
                var failedListener = (Socket)typeof(AsyncServer).GetField("_listenerSocket", flags)!.GetValue(failedServer)!;
                using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await client.ConnectAsync(failedListener.LocalEndPoint!);
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                Check(await client.ReceiveAsync(new byte[1], SocketFlags.None, timeout.Token) == 0,
                    "Constructor failure closes accepted socket");
                await failedServer.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(3));
                await failing;
                Check(failedServer.GetActiveConnections() == 0, "Constructor failure does not retain active connection");
            }
            finally { await failedServer.DisposeAsync(); }
            Check(AsyncServer.AcceptRetryDelay(1) == TimeSpan.FromMilliseconds(100)
                && AsyncServer.AcceptRetryDelay(100) == TimeSpan.FromMilliseconds(3200), "Accept backoff is bounded");
            string metrics = NetworkMetrics.Snapshot();
            Check(metrics.Contains("socketQueuedKiB=") && metrics.Contains("disconnects=["), "Network measurements available to stats command");
            string snapshotPath = Path.Combine(Path.GetTempPath(), "NetGuardLiveStats-" + Guid.NewGuid().ToString("N"), "network.stats.txt");
            await using var sampler = new LiveNetworkStats(snapshotPath, TimeSpan.FromMilliseconds(50));
            using var snapshotDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            while (!File.Exists(snapshotPath)) await Task.Delay(10, snapshotDeadline.Token);
            string initial = await File.ReadAllTextAsync(snapshotPath);
            Check(initial.Contains("pid=") && initial.Contains("live=") && initial.Contains("[network]"), "Live snapshot contains process identity and metrics");
            while (await File.ReadAllTextAsync(snapshotPath) == initial) await Task.Delay(10, snapshotDeadline.Token);
            Check(true, "Live snapshot refreshes without console commands");
        }
        finally
        {
            await proxy.DisposeAsync();
            foreach (var socket in clients.Concat(servers)) socket.Dispose();
            upstream.Stop(); App.Module = previousModule; FilterRegistry.Loader = previousLoader;
        }
        Console.WriteLine($"PASS: {checks} connection lifecycle checks.");
        return checks;
    }
}
