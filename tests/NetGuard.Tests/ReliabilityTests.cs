using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Module.Config;
using Module.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SilkroadSecurityAPI;
using App = Module.Main;

internal static class ReliabilityTests
{
    internal static async Task<int> Run(string work)
    {
        int checks = 0;
        void Check(bool value, string name) { if (!value) throw new Exception(name); checks++; }
        byte[] Frame(params byte[] body)
        {
            var wire = new byte[body.Length + 6];
            BinaryPrimitives.WriteUInt16LittleEndian(wire, (ushort)body.Length);
            BinaryPrimitives.WriteUInt16LittleEndian(wire.AsSpan(2), 0x600D);
            body.CopyTo(wire, 6); return wire;
        }
        void Feed(Security security, params byte[] payload)
        { var bytes = Frame(payload); security.Recv(bytes, 0, bytes.Length); }
        void Reject(Action action, string name)
        {
            try { action(); } catch (SilkroadSecurityAPI.SecurityException) { Check(true, name); return; }
            throw new Exception(name);
        }
        Reject(() => Feed(new Security(), 1, 0, 0, 0x13, 0x30), "Zero massive count rejected");
        Reject(() => Feed(new Security(), 2), "Unknown massive mode rejected");
        Reject(() => Feed(new Security(), 0, 1), "Orphan fragment rejected");
        Reject(() => Feed(new Security(), 1, 1, 0, 0, 0x50), "Encapsulated handshake rejected");
        var nested = new Security(); Feed(nested, 1, 1, 0, 0x13, 0x30);
        Reject(() => Feed(nested, 1, 1, 0, 0x13, 0x30), "Nested massive begin rejected");
        var limited = new Security { MaxMassivePacketBytes = 3 };
        Feed(limited, 1, 2, 0, 0x13, 0x30); Feed(limited, 0, 1, 2);
        Reject(() => Feed(limited, 0, 3, 4), "Cumulative assembly bytes limited before append");
        var expiring = new Security { MassivePacketTimeout = TimeSpan.FromMilliseconds(20) };
        Feed(expiring, 1, 1, 0, 0x13, 0x30); await Task.Delay(40);
        Reject(expiring.CheckReceiveTimeout, "Idle massive assembly expires");

        var admission = new ConnectionAdmission(2, 1);
        Check(admission.TryAcquire("one") && !admission.TryAcquire("one") && admission.TryAcquire("two")
            && !admission.TryAcquire("three"), "Global and per-IP limits enforced");
        admission.Release("one");
        Check(admission.TryAcquire("three"), "Disconnect releases capacity");

        string file = Path.Combine(work, "settings-test.json");
        var current = new AppSettings();
        string valid = JsonConvert.SerializeObject(current);
        Check(SettingsFile.Parse(valid).ServerType.CurrentValue == "jSRO", "Default settings schema is valid");
        File.WriteAllText(file, valid);
        using (var reloader = new SettingsReloader(file, value => current = value, _ => { }))
        {
            var previous = current;
            foreach (string invalid in new[] { "{", "{}", "null", valid.Replace("\"jSRO\"", "\"unsupported\"") })
            {
                File.WriteAllText(file, invalid);
                Check(!reloader.ReloadNow() && ReferenceEquals(previous, current), "Invalid settings retain last good snapshot");
            }
            var updated = JObject.Parse(valid); updated["DisableConsoleLogging"] = true;
            File.WriteAllText(file, updated.ToString());
            Check(reloader.ReloadNow() && current.DisableConsoleLogging, "Complete settings publish successfully");
            var replacement = file + ".new";
            updated["DisableConsoleLogging"] = false;
            File.WriteAllText(replacement, updated.ToString());
            File.Move(replacement, file, true);
            using var wait = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            while (current.DisableConsoleLogging) await Task.Delay(20, wait.Token);
            Check(true, "Atomic file replacement triggers debounced reload");
        }

        var previousModule = App.Module;
        var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
        var front = new TcpListener(IPAddress.Loopback, 0); front.Start();
        using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var connect = client.ConnectAsync((IPEndPoint)front.LocalEndpoint);
        var accepted = await front.AcceptSocketAsync(); await connect; front.Stop();
        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        App.Module = new ModuleSettings { ModuleIP = "127.0.0.1", ModulePort = ((IPEndPoint)listener.LocalEndpoint).Port,
            HandshakeTimeoutSeconds = 1, LoginTimeoutSeconds = 2, DrainTimeoutSeconds = 1 };
        var proxy = new Module.Networking.Module(987660, accepted,
            (int id, ref Socket socket, ModuleType type) => { socket.Dispose(); closed.TrySetResult(); }, ModuleType.AgentModule);
        try
        {
            var starting = proxy.StartAsync();
            using var upstream = await listener.AcceptSocketAsync(); listener.Stop(); await starting;
            await closed.Task.WaitAsync(TimeSpan.FromSeconds(4));
            Check(true, "Silent client handshake deadline closes connection");
        }
        finally { proxy.Stop(); listener.Stop(); App.Module = previousModule; }
        return checks;
    }
}
