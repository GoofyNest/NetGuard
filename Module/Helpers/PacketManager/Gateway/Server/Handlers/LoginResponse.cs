using Module.Engine.Classes;
using Module.Framework;
using Module.Services;
using SilkroadSecurityAPI;
using static Module.Helpers.PacketManager.Gateway.Opcodes.Server;

namespace Module.Helpers.PacketManager.Gateway.Server.Handlers
{
    public class LoginResponse : IPacketHandler
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            response.ModifiedPacket = null!;

            byte res = packet.ReadUInt8();

            if (res == 1)
            {
                uint id = packet.ReadUInt32();
                string host = packet.ReadAscii();
                int port = packet.ReadUInt16();

                var index = Main._config.ModuleBinding.FindIndex(m => m.moduleIP == host && m.modulePort == port);
                if (index == -1)
                {
                    Custom.WriteLine("Could not find agent bindings", ConsoleColor.Red);
                    return response;
                }

                // If agent bindings are found, create the modified packet
                var guardModule = Main._config.ModuleBinding[index];

                Custom.WriteLine($"Using {guardModule.guardIP} {guardModule.guardPort}", ConsoleColor.Cyan);

                var modifiedPacket = new Packet(SERVER_GATEWAY_LOGIN_RESPONSE, true);
                modifiedPacket.WriteUInt8(res);
                modifiedPacket.WriteUInt32(id);
                modifiedPacket.WriteAscii(guardModule.guardIP);
                modifiedPacket.WriteUInt16(guardModule.guardPort);
                modifiedPacket.WriteUInt32(0);  // Add any other modifications you need
                // modifiedPacket.Lock(); Not sure if needed

                response.SendImmediately = false;

                response.ModifiedPacket = modifiedPacket;

                return response; // Return true to indicate we modified the packet


            }

            return response;
        }
    }
}
