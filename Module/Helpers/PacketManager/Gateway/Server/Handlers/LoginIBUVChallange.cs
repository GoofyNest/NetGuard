using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using static Module.Classes.Settings;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Client;

namespace Module.Helpers.PacketManager.Gateway.Server.Handlers
{
    public class LoginIBUVChallange : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var settings = Main._settings.Gateway.LoginCaptcha;

            if (!settings.DisableCaptcha)
                return response;

            var modified = new Packet(GATEWAY_LOGIN_IBUV_ANSWER, false);
            modified.WriteAscii(settings.StaticCaptchaCode);

            response.ModifiedPackets.Add(new PacketList { Packet = modified, SendImmediately = true, securityType = SecurityType.RemoteSecurity });

            return response;
        }
    }
}
