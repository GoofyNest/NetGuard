using Module.Networking;
using Module.PacketHandler;
using Module.PacketHandler.Filtering;
using Module.Services;
using NetGuard.Hot.Helpers;
using NetGuard.Hot;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering.Agent.Client
{
    public sealed class Landing : ILanding
    {
        public PacketHandlingResult? Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            // ---- Offline / char-selection state machine ----
            if (!client.Agent.IsIngame)
            {
                if (client.Agent.InCharSelectionScreen)
                {
                    switch (packet.Opcode)
                    {
                        case (ushort)Global.KeepAlive:
                        case (ushort)CharScreen.SelectionRename:
                        case (ushort)CharScreen.SelectionJoin:
                        case (ushort)CharScreen.SelectionAction:
                            if (packet.Opcode == (ushort)CharScreen.SelectionJoin
                                && !client.Agent.SentJoinRequest)
                            {
                                client.PlayerInfo.CurrentCharName = packet.ReadAscii();
                            }
                            return new PacketHandlingResult { };
                    }

                    Custom.WriteLine(
                        $"Prevented {client.PlayerInfo.IpInfo.Ip}, unknown packet in char selection",
                        ConsoleColor.Yellow);
                    return new PacketHandlingResult { ResultType = PacketResultType.Block };
                }

                switch (packet.Opcode)
                {
                    case (ushort)Authentication.Auth:
                    case (ushort)Global.Identification:
                    case (ushort)Global.AcceptHandshake:
                    case (ushort)Global.Handshake:       // ← was Global.HandShake
                        return new PacketHandlingResult { };

                    default:
                        Custom.WriteLine(
                            $"Prevented {client.PlayerInfo.IpInfo.Ip}, in-game packet while offline",
                            ConsoleColor.Yellow);
                        return new PacketHandlingResult { ResultType = PacketResultType.Block };
                }
            }

            if (PacketType.BadOpcodes.Contains(packet.Opcode))
                return new PacketHandlingResult { ResultType = PacketResultType.Block };

            if (PacketDiagnostics.Enabled && !PacketType.GoodOpcodes.Contains(packet.Opcode))
            {
                Custom.WriteLine(
                    $"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]" +
                    $"{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}" +
                    $"{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}",
                    ConsoleColor.Magenta);
            }

            switch (packet.Opcode)
            {
                case (ushort)Authentication.Auth:
                case (ushort)Global.Identification:
                case (ushort)Global.AcceptHandshake:
                case (ushort)Global.Handshake:
                    if (client.Agent.InCharSelectionScreen || client.Agent.IsIngame)
                    {
                        Custom.WriteLine(
                            $"Prevented {client.PlayerInfo.IpInfo.Ip}, login packet after login",
                            ConsoleColor.Yellow);
                        return new PacketHandlingResult { ResultType = PacketResultType.Block };
                    }
                    return new PacketHandlingResult { };
            }

            switch (packet.Opcode)
            {
                case (ushort)CharScreen.SelectionRename:
                case (ushort)CharScreen.SelectionJoin:
                case (ushort)CharScreen.SelectionAction:
                    if (!client.Agent.InCharSelectionScreen || client.Agent.IsIngame)
                    {
                        Custom.WriteLine(
                            $"Prevented {client.PlayerInfo.IpInfo.Ip}, char-screen packet out of place",
                            ConsoleColor.Yellow);
                        return new PacketHandlingResult { ResultType = PacketResultType.Block };
                    }
                    return new PacketHandlingResult { };
            }

            // Permit the loading acknowledgements and keepalive; reject gameplay
            // until the server reports that teleport has completed.
            if (client.Host()!.Agent.IsTeleporting && packet.Opcode is not (0x34B6 or 0x3012 or 0x3014 or 0x2002))
            {
                Custom.WriteLine(
                    $"[Teleport] Blocked C->S opcode=0x{packet.Opcode:X4} character={client.PlayerInfo.CurrentCharName}");
                return new PacketHandlingResult { ResultType = PacketResultType.Block };
            }

            // ---- Exchange window ----
            if (client.Agent.InExchange)
            {
                switch (packet.Opcode)
                {
                    case 0x7034:
                    {
                        byte status = packet.ReadUInt8();

                        if(status == 0x0D || status == 0x04 || status == 0x05)
                        {
                            return new PacketHandlingResult { };
                        }
                        else
                        {
                            ctx.SendToClient(ChatPackets.CreateNotice(
                                $"[exchange] 0x{packet.Opcode:X4} is not allowed while trading."));
                            return new PacketHandlingResult { ResultType = PacketResultType.Block };
                        }
                    }
                    break;

                    case (ushort)Exchange.Confirm:
                    case (ushort)Exchange.Cancel:
                    case (ushort)Exchange.Approve:
                    case (ushort)Global.KeepAlive:      // ← was Global.Ping
                        return new PacketHandlingResult { };

                    default:
                        ctx.SendToClient(ChatPackets.CreateNotice(
                            $"[exchange] 0x{packet.Opcode:X4} is not allowed while trading."));
                        return new PacketHandlingResult { ResultType = PacketResultType.Block };
                }
            }

            return new PacketHandlingResult { };
        }
    }
}
