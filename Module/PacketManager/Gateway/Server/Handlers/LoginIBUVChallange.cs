﻿using System;
using Module.Engine.Classes;
using Module.Framework;
using SilkroadSecurityAPI;
using static Module.Classes.Settings;
using static Module.PacketManager.Gateway.Opcodes.Client;
using static Module.PacketManager.Gateway.Opcodes.Server;

namespace Module.PacketManager.GatewayModule.Server.Handlers
{
    public class LoginIBUVChallange : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            IBuvChallange settings = Main._settings.gatewaySettings.captchaSettings;

            response.ModifiedPacket = null!;

            if (!settings.bypassCaptcha)
                return response;

            var modifiedPacket = new Packet(GATEWAY_LOGIN_IBUV_ANSWER, false);
            modifiedPacket.WriteAscii(settings.captchaCode);

            response.securityType = SecurityType.RemoteSecurity;
            response.SendImmediately = true;
            response.ModifiedPacket = modifiedPacket;

            return response;
        }
    }
}
