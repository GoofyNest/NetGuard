using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Module.Config;

internal static class SettingsFile
{
    internal static AppSettings Parse(string text)
    {
        var root = JObject.Parse(text, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
        JToken Require(string path, JTokenType type)
        {
            var token = root.SelectToken(path);
            if (token?.Type != type) throw new JsonException($"Missing or invalid setting: {path}");
            return token;
        }
        foreach (string path in new[] { "DisableLogWriting", "DisableConsoleLogging",
            "Agent.GameMasterConfig.EnablePermissionSystem", "Agent.GameMasterConfig.Misc.DisableGMChat",
            "Agent.GameMasterConfig.Misc.DisableGMConsole", "Agent.GameMasterConfig.Misc.SpawnVisible",
            "Gateway.LoginCaptcha.DisableCaptcha" }) Require(path, JTokenType.Boolean);
        string version = (string)Require("ServerType.CurrentValue", JTokenType.String)!;
        if (!Enum.GetNames<FileVersion>().Contains(version)) throw new JsonException("Unsupported ServerType.CurrentValue.");
        if (string.IsNullOrWhiteSpace((string?)Require("Data.Path", JTokenType.String)))
            throw new JsonException("Data.Path must not be empty.");
        var captcha = (string?)Require("Gateway.LoginCaptcha.StaticCaptchaCode", JTokenType.String);
        if ((bool)root.SelectToken("Gateway.LoginCaptcha.DisableCaptcha")! && string.IsNullOrWhiteSpace(captcha))
            throw new JsonException("Static captcha code must not be empty when enabled.");
        var operators = (JArray)Require("Agent.GameMasterConfig.GMs", JTokenType.Array);
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in operators)
        {
            if (entry is not JObject gm || gm["Username"]?.Type != JTokenType.String
                || string.IsNullOrWhiteSpace((string?)gm["Username"]) || !names.Add((string)gm["Username"]!)
                || gm["ShouldSpawnVisible"]?.Type != JTokenType.Boolean || gm["NoPinkChat"]?.Type != JTokenType.Boolean
                || gm["Permissions"] is not JArray permissions
                || permissions.Any(p => p.Type != JTokenType.String || string.IsNullOrWhiteSpace((string?)p)))
                throw new JsonException("Invalid or duplicate GM configuration.");
        }
        return root.ToObject<AppSettings>() ?? throw new JsonException("Empty settings.");
    }
}

internal sealed class SettingsReloader : IDisposable
{
    private readonly object _sync = new();
    private readonly string _path;
    private readonly Action<AppSettings> _publish;
    private readonly Action<Exception> _failed;
    private readonly Timer _timer;
    private readonly FileSystemWatcher _watcher;
    private bool _disposed;

    internal SettingsReloader(string path, Action<AppSettings> publish, Action<Exception> failed)
    {
        _path = Path.GetFullPath(path); _publish = publish; _failed = failed;
        _timer = new Timer(_ => ReloadNow(), null, Timeout.Infinite, Timeout.Infinite);
        _watcher = new FileSystemWatcher(Path.GetDirectoryName(_path)!, Path.GetFileName(_path))
        { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size };
        _watcher.Changed += (_, _) => Schedule();
        _watcher.Created += (_, _) => Schedule();
        _watcher.Renamed += (_, _) => Schedule();
        _watcher.Error += (_, e) => { _failed(e.GetException()); Schedule(); };
        _watcher.EnableRaisingEvents = true;
    }

    private void Schedule()
    {
        lock (_sync) if (!_disposed) _timer.Change(300, Timeout.Infinite);
    }

    internal bool ReloadNow()
    {
        lock (_sync)
        {
            if (_disposed) return false;
            try
            {
                using var stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
                using var reader = new StreamReader(stream);
                var settings = SettingsFile.Parse(reader.ReadToEnd());
                _publish(settings);
                return true;
            }
            catch (Exception ex) { _failed(ex); return false; }
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            _disposed = true;
            _watcher.Dispose();
            _timer.Dispose();
        }
    }
}
