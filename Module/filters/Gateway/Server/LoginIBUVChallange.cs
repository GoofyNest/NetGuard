namespace NetGuard.Filters.Gateway.Server
{
    [Filter(0x2322)] // SERVER_GATEWAY_LOGIN_IBUV_CHALLENGE
    public sealed class LoginIBUVChallange : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            var settings = Main.Settings.Gateway.LoginCaptcha;

            if (!settings.DisableCaptcha)
                return response;

            var modified = new Packet(0x6323, false); // GATEWAY_LOGIN_IBUV_ANSWER
            modified.WriteAscii(settings.StaticCaptchaCode);

            response.ModifiedPackets.Add(new PacketList
            {
                Packet = modified,
                SendImmediately = true,
                SecurityType = SecurityType.RemoteSecurity
            });

            return response;
        }
    }
}