namespace NetGuard.Filters.Agent.Client
{
    internal static class HandshakeShared
    {
        public static PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            return new PacketHandlingResult
            {
                SkipSending = false,
                ResultType = PacketResultType.SkipSending
            };
        }
    }
}