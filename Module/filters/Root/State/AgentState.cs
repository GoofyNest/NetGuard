using Module.PacketHandler.Filtering;
using SilkroadSecurityAPI;

namespace NetGuard.Hot;

public sealed class AgentState : ISessionState
{
    public List<CharState> Chars { get; } = new();
    public Packet? CharData { get; set; }
    public int CharDataProcess { get; set; }

    public CharState? Current(string name)
        => Chars.FirstOrDefault(c => c.Charname == name);

    public CharState EnsureCurrent(string name)
    {
        var c = Current(name);
        if (c is null)
        {
            c = new CharState { Charname = name };
            Chars.Add(c);
        }
        return c;
    }
}