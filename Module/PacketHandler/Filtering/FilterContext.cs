namespace Module.PacketHandler.Filtering
{
    public enum FilterContext
    {
        AgentClient,
        AgentServer,
        GatewayClient,
        GatewayServer
    }

    public static class FilterContextExtensions
    {
        public static string ToFolderPath(this FilterContext ctx) => ctx switch
        {
            FilterContext.AgentClient => Path.Combine("Agent", "Client"),
            FilterContext.AgentServer => Path.Combine("Agent", "Server"),
            FilterContext.GatewayClient => Path.Combine("Gateway", "Client"),
            FilterContext.GatewayServer => Path.Combine("Gateway", "Server"),
            _ => throw new ArgumentOutOfRangeException(nameof(ctx))
        };
    }
}