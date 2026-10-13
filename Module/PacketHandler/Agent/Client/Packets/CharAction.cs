using Module.Config;
using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Agent.Client.Packets
{
    public class CharAction : IPacketHandler
    {
        public enum ActionCommandType : byte
        {
            Execute = 1,
            Cancel = 2,
        }

        public enum ActionType : byte
        {
            Attack = 1,
            Pickup = 2,
            Trace = 3,
            Cast = 4,
            Dispel = 5,
        }

        public enum ActionTarget : byte
        {
            None = 0,
            Entity = 1,
            Area = 2,
        }

        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            PacketHandlingResult response = new();

            var _files = Main.Settings.ServerType.CurrentValue;

            if (_files != "jSRO")
                return response;

            Packet modified = new Packet(0x7074);

            /*
                Send trace request

                var packet = new Packet(0x7074);
                packet.WriteByte(1);
                packet.WriteByte(3);
                packet.WriteByte(1);
                packet.WriteUInt(player.UniqueId);
            */

            /*
                Picup item

                var packet = new Packet(0x7074);
                packet.WriteByte(ActionCommandType.Execute);
                packet.WriteByte(ActionType.Pickup);
                packet.WriteByte(ActionTarget.Entity);
                packet.WriteUInt(UniqueId);

                [C -> S][7074]
                01                                                ................
                02                                                ................
                01                                                ................
                01                                                ................
                A5 2F 01 00                                       ./..............

                [C -> S][7074]
                01                                                ................
                02                                                ................
                01                                                ................
                01                                                ................
                09 2E 01 00                                       ................

            */

            /*
                Cast skill

                var packet = new Packet(0x7074);
                packet.WriteByte(ActionCommandType.Execute); //Execute
                packet.WriteByte(ActionType.Cast); //Use Skill
                packet.WriteUInt(skill.Id);
                packet.WriteByte(ActionTarget.Entity);
            */

            // AddAction(new byte[] { 0x01, 0x02, 0x01, 0x61, 0x35, 0x01, 0x00 });
            ActionCommandType CommandType = (ActionCommandType)packet.ReadUInt8();
            switch(CommandType)
            {
                case ActionCommandType.Execute:
                    {
                        ActionType ActionType = (ActionType)packet.ReadUInt8(); // 0x02

                        switch (ActionType)
                        {
                            case ActionType.Pickup:
                                {
                                    ActionTarget TargetType = (ActionTarget)packet.ReadUInt8(); // 0x01

                                    byte malformedByte = packet.ReadUInt8(); // should be 0x01

                                    if (malformedByte > 2)
                                    {
                                        try
                                        {
                                            modified.WriteUInt8((byte)CommandType);
                                            modified.WriteUInt8((byte)ActionType);
                                            modified.WriteUInt8((byte)TargetType); // spoofed TargetType
                                            modified.WriteUInt8(0x01); // missing byte

                                            // Unique ID
                                            modified.WriteUInt8(malformedByte);
                                            modified.WriteUInt8(packet.ReadUInt8());
                                            modified.WriteUInt8(packet.ReadUInt8());
                                            modified.WriteUInt8(packet.ReadUInt8());

                                            response.ModifiedPackets.Add(new PacketList
                                            {
                                                Packet = modified,
                                                SendImmediately = true,
                                                SecurityType = SecurityType.RemoteSecurity
                                            });
                                            return response;
                                        }
                                        catch
                                        {
                                            return response; // block on crash
                                        }
                                    }
                                }

                                break;

                        }
                        
                    }
                    break;

                case ActionCommandType.Cancel:
                    {
                        modified.WriteUInt8(0x02);
                        response.ModifiedPackets.Add(new PacketList
                        {
                            Packet = modified,
                            SendImmediately = true,
                            SecurityType = SecurityType.RemoteSecurity
                        });
                        return response;
                    }

                default:
                    {
                        return response; // block packet
                    }
                    break;
            }



            return response;
        }
    }
}
