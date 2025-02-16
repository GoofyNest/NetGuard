using Module.Networking;
using SilkroadSecurityAPI;
using static Module.PacketHandler.Gateway.Opcodes.Client;

namespace Module.PacketHandler.Gateway.Server.Packets
{
    public class LoginIBUVChallange : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var settings = Main.Settings.Gateway.LoginCaptcha;

            if (!settings.DisableCaptcha)
                return response;

            var modified = new Packet(GATEWAY_LOGIN_IBUV_ANSWER, false);
            modified.WriteAscii(settings.StaticCaptchaCode);

            response.ModifiedPackets.Add(new PacketList { Packet = modified, SendImmediately = true, SecurityType = SecurityType.RemoteSecurity });

            return response;
        }
    }
}
