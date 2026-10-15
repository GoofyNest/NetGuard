using Module.Config;
using Module.Networking;
using Module.PacketHandler.Agent.Client.Packets;
using Module.PacketHandler.Filtering;
using Module.Services;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client
{
    public class PacketHandler
    {
        public static IPacketHandler GetHandler(Packet packet, SessionData client)
        {
            //Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Magenta);

            /*
                New way to block exploits

                This code will force people to login to send the rest of the Client -> Server packets
            */
            if (client.Agent.IsIngame == false)
            {
                /*
                    Block all known / unknown SR_ShardManager exploits
                    https://www.elitepvpers.com/forum/sro-pserver-guides-releases/4232366-release-disconnect-players-exploit-found-iwa.html

                    This code will limit people from sending unknown packets whilst in the Char selection screen
                */
                if (client.Agent.InCharSelectionScreen)
                {
                    switch (packet.Opcode)
                    {
                        case (ushort)Global.Ping:
                        case (ushort)CharScreen.SelectionRename:
                        case (ushort)CharScreen.SelectionJoin:
                        case (ushort)CharScreen.SelectionAction:
                            {
                                // These packets are allowed in character selection.
                                if (packet.Opcode == (ushort)CharScreen.SelectionJoin)
                                {
                                    if (!client.Agent.SentJoinRequest)
                                        client.PlayerInfo.CurrentCharName = packet.ReadAscii(); // Temp storing char name, check CharacterJoin.cs in Server handler
                                }

                                // Do nothing, allow this
                                return null!;
                            }

                    }

                    // Other packets are dropped in character selection
                    Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending unknown packets whilst in char selection", ConsoleColor.Yellow);
                    return new DefaultBlock();
                }

                switch (packet.Opcode)
                {
                    case (ushort)Authentication.Auth:
                    case (ushort)Global.Identification:
                    case (ushort)Global.AcceptHandshake:
                    case (ushort)Global.HandShake:
                        // Filters handle these if registered; otherwise forward.
                        return FilterRegistry.Resolve(FilterContext.AgentClient, packet.Opcode) is { } df1
                            ? new DynamicFilterAdapter(df1)
                            : null!;
                }

                Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending inGame packets whilst offline", ConsoleColor.Yellow);
                return new DefaultBlock();
            }

            if (PacketType.BadOpcodes.Contains(packet.Opcode))
            {
                return new DefaultBlock(); // Block the exploit
            }

            if (!PacketType.GoodOpcodes.Contains(packet.Opcode))
            {
                Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Magenta);
            }

            // Block login packets whilst playing (not normal behaviour)
            switch (packet.Opcode)
            {
                case (ushort)Authentication.Auth:
                case (ushort)Global.Identification:
                case (ushort)Global.AcceptHandshake:
                case (ushort)Global.HandShake:
                    {
                        if (client.Agent.InCharSelectionScreen)
                        {
                            Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending login packets whilst in char screen", ConsoleColor.Yellow);
                            return new DefaultBlock();
                        }

                        if (client.Agent.IsIngame)
                        {
                            Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending login packets whilst in-game", ConsoleColor.Yellow);
                            return new DefaultBlock();
                        }
                        return null!;
                    }
            }

            // Block char screen packets whilst playing (not normal behaviour)
            switch (packet.Opcode)
            {
                case (ushort)CharScreen.SelectionRename:
                case (ushort)CharScreen.SelectionJoin:
                case (ushort)CharScreen.SelectionAction:
                    {
                        if (!client.Agent.InCharSelectionScreen)
                        {
                            Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending char screen packets when not logged in", ConsoleColor.Yellow);
                            return new DefaultBlock();
                        }

                        if (client.Agent.IsIngame)
                        {
                            Custom.WriteLine($"Prevented {client.PlayerInfo.IpInfo.Ip}, Sending char screen packets when in-game", ConsoleColor.Yellow);
                            return new DefaultBlock();
                        }
                        return null!;
                    }
            }

            // All security gates passed. Now dispatch to a dynamic filter (if any).
            var dynamicFilter = FilterRegistry.Resolve(FilterContext.AgentClient, packet.Opcode);
            if (dynamicFilter != null)
                return new DynamicFilterAdapter(dynamicFilter);

            // No filter matched — forward as-is.
            return null!;
        }
    }
}