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

            byte res = packet.ReadUInt8();

            if (res == 1)
            {
                uint id = packet.ReadUInt32();
                string host = packet.ReadAscii();
                int port = packet.ReadUInt16();

                var index = Main.Config.ModuleBinding.FindIndex(m => m.ModuleIP == host && m.ModulePort == port);
                if (index == -1)
                {
                    Custom.WriteLine("Could not find agent bindings", ConsoleColor.Red);
                    return response;
                }

                // If agent bindings are found, create the modified packet
                var guardModule = Main.Config.ModuleBinding[index];

                Custom.WriteLine($"Using {guardModule.GuardIP} {guardModule.GuardPort}", ConsoleColor.Cyan);

                var modified = new Packet(SERVER_GATEWAY_LOGIN_RESPONSE, true);
                modified.WriteUInt8(res);
                modified.WriteUInt32(id);
                modified.WriteAscii(guardModule.GuardIP);
                modified.WriteUInt16(guardModule.GuardPort);
                modified.WriteUInt32(0);  // Add any other modifications you need
                // modifiedPacket.Lock(); Not sure if needed

                response.ModifiedPackets.Add(new PacketList { Packet = modified });
                return response; // Return true to indicate we modified the packet


            }

            return response;
        }
    }
}
