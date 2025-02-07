using System;
using Framework;
using Module;
using NetGuard.Engine.Classes;
using NetGuard.Services;
using SilkroadSecurityAPI;
using static Framework.Opcodes.Server;

namespace PacketManager.Server.Handlers
{
    public class ServerLoginResponseHandler : IPacketHandler
    {
        public bool Handle(Packet packet, SessionData client, out Packet modifiedPacket)
        {
            modifiedPacket = null;

            byte response = packet.ReadUInt8();

            if(response == 1)
            {
                uint id = packet.ReadUInt32();
                string host = packet.ReadAscii();
                int port = packet.ReadUInt16();

                var index = Main._config._agentModules.FindIndex(m => m.moduleIP == host && m.modulePort == port);
                if (index == -1)
                {
                    Custom.WriteLine("Could not find agent bindings", ConsoleColor.Red);
                    return true;
                }

                // If agent bindings are found, create the modified packet
                var guardModule = Main._config._agentModules[index];

                Custom.WriteLine($"Using {guardModule.guardIP} {guardModule.guardPort}", ConsoleColor.Cyan);

                modifiedPacket = new Packet(SERVER_GATEWAY_LOGIN_RESPONSE, true);
                modifiedPacket.WriteUInt8(response);
                modifiedPacket.WriteUInt32(id);
                modifiedPacket.WriteAscii(guardModule.guardIP);
                modifiedPacket.WriteUInt16(guardModule.guardPort);
                modifiedPacket.WriteUInt32(0);  // Add any other modifications you need
                // modifiedPacket.Lock(); Not sure if needed

                return true; // Return true to indicate we modified the packet


            }

            return false;
        }
    }
}
