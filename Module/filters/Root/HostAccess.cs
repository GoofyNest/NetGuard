using Module.Networking;

namespace NetGuard.Hot;

public static class HostAccess
{
    public static SessionHost? Host(this SessionData s) => s.Host as SessionHost;

    public static CharState? CurrentChar(this SessionData s)
        => s.Host()?.Agent.Current(s.PlayerInfo.CurrentCharName);
}