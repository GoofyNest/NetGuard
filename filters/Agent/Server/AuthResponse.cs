namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xA103)] // SERVER_AGENT_AUTH_RESPONSE
    public sealed class AuthResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            byte errorCode = packet.ReadUInt8();

            if (errorCode == 0x01)
                client.Agent.InCharSelectionScreen = true;

            Custom.WriteLine($"SERVER_AGENT_AUTH_RESPONSE {errorCode}", ConsoleColor.Cyan);

            return response;
        }
    }
}