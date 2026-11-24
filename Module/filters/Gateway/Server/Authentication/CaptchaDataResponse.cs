/*
0x2322 | Gateway | Server -> Client
Name: CaptchaDataResponse

*/

namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0x2322)] // SERVER_GATEWAY_LOGIN_IBUV_CHALLENGE
    public sealed class CaptchaDataResponse : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            try
            {
                var response = new PacketHandlingResult();
                var settings = Main.Settings.Gateway.LoginCaptcha;
                if (!settings.DisableCaptcha)
                    return response;
                var modified = new Packet(0x6323, false); // GATEWAY_LOGIN_IBUV_ANSWER
                modified.WriteAscii(settings.StaticCaptchaCode);
                response.ModifiedPackets.Add(new PacketList { Packet = modified, SendImmediately = true, SecurityType = SecurityType.RemoteSecurity });
                return response;
            }
            catch (Exception ex)
            {
                Custom.WriteLine("[S->C] 0x2322 handler error: " + ex.Message, ConsoleColor.Red);
                return new PacketHandlingResult();
            }
        }
    }
}