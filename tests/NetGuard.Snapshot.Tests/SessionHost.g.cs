namespace NetGuard.Hot;
public sealed class SessionHost
{
    public AgentState Agent { get; } = new();
    public GatewayState Gateway { get; } = new();
    public UseItem UseItem { get; } = new();
}
