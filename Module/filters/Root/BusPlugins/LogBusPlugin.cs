using Module.Networking;
using Module.PacketHandler.Filtering;
using System.Text.Json;

namespace NetGuard.Hot.BusPlugins;

public sealed class LogBusPlugin : IBusPlugin
{
    public void Register(BusDispatcher dispatcher)
    {
        dispatcher.On("agent.quit_acknowledged", (session, msg) =>
        {
            if (!MatchesUser(session, msg)) return;
            var user = msg.Payload is JsonElement p && p.TryGetProperty("Username", out var u)
                ? u.GetString() : "?";
            var target = msg.Payload is JsonElement p2 && p2.TryGetProperty("Target", out var t)
                ? t.GetString() : "?";

            Custom.WriteLine($"[gateway] {user} quit acknowledged (ip={session.PlayerInfo.IpInfo.Ip})", ConsoleColor.Cyan);
        });

        dispatcher.On("gateway.login_requested", (session, msg) =>
        {
            if (!MatchesUser(session, msg)) return;
            if (msg.Payload is not JsonElement body
                || !body.TryGetProperty("Target", out var destination)
                || destination.ValueKind != JsonValueKind.String
                || !string.Equals(destination.GetString(), $"{Main.Module.ModuleIP}:{Main.Module.ModulePort}", StringComparison.OrdinalIgnoreCase))
                return;
            var user = msg.Payload is JsonElement p && p.TryGetProperty("Username", out var u)
                ? u.GetString() : "?";
            var target = msg.Payload is JsonElement p2 && p2.TryGetProperty("Target", out var t)
                ? t.GetString() : "?";

            session.Agent.LoginRequested = true;
            Custom.WriteLine($"[agent] login requested for {user} -> {target}", ConsoleColor.Cyan);
        }); 
    }

    private static bool MatchesUser(SessionData session, BusMessage msg)
        => !string.IsNullOrWhiteSpace(session.PlayerInfo.Username)
        && msg.Payload is JsonElement body && body.ValueKind == JsonValueKind.Object
        && body.TryGetProperty("Username", out var user) && user.ValueKind == JsonValueKind.String
        && string.Equals(user.GetString(), session.PlayerInfo.Username, StringComparison.OrdinalIgnoreCase);
}
