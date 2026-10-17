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
            var user = msg.Payload is JsonElement p && p.TryGetProperty("Username", out var u)
                ? u.GetString() : "?";
            var target = msg.Payload is JsonElement p2 && p2.TryGetProperty("Target", out var t)
                ? t.GetString() : "?";

            Custom.WriteLine($"[gateway] {user} quit acknowledged (ip={session.PlayerInfo.IpInfo.Ip})", ConsoleColor.Cyan);
        });

        dispatcher.On("gateway.login_requested", (session, msg) =>
        {
            var user = msg.Payload is JsonElement p && p.TryGetProperty("Username", out var u)
                ? u.GetString() : "?";
            var target = msg.Payload is JsonElement p2 && p2.TryGetProperty("Target", out var t)
                ? t.GetString() : "?";

            session.Agent.LoginRequested = true;
            Custom.WriteLine($"[agent] login requested for {user} -> {target}", ConsoleColor.Cyan);
        }); 
    }
}