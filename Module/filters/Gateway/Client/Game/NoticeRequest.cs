/*
0x6104 | Gateway | Client -> Server
Name: NoticeRequest

// Payload fields are not identified by the available source.

*/

namespace NetGuard.Filters.Gateway.Client
{
    [Filter(0x6104)]
    public sealed class NoticeRequest : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            if (packet.GetBytes().Length > 1)
                response.ResultType = PacketResultType.Block;

            Custom.WriteLine($"CLIENT_GATEWAY_NOTICE_REQUEST {packet.GetBytes().Length}", ConsoleColor.Magenta);

            return response;
        }
    }
}