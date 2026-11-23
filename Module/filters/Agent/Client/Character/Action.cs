using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7074)]
    public sealed class PlayerAction : IFilter
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

        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            Custom.WriteLine($"[C->S] [{packet.Opcode:X4}][{packet.GetBytes().Length} bytes]{(packet.Encrypted ? "[Encrypted]" : "")}{(packet.Massive ? "[Massive]" : "")}{Environment.NewLine}{Utility.HexDump(packet.GetBytes())}{Environment.NewLine}", ConsoleColor.Magenta);
            var response = new PacketHandlingResult();

            var h = client.Host()!;
            
            var _files = Main.Settings.ServerType.CurrentValue;

            if (_files != "jSRO")
                return response;

            Packet modified = new Packet(0x7074);

            ActionCommandType CommandType = (ActionCommandType)packet.ReadUInt8();
            switch (CommandType)
            {
                case ActionCommandType.Execute:
                    {
                        ActionType ActionType = (ActionType)packet.ReadUInt8();

                        int packetLength = packet.GetBytes().Length;

                        switch (ActionType)
                        {
                            /*
                                [C -> S][7074]
                                01                                                ................
                                01                                                ................
                                01                                                ................
                                01                                                ................
                                60 3C 00 00                                       `<..............
                            */
                            case ActionType.Attack: // 0x01
                                {
                                    if(packetLength != 8)
                                    {
                                        Custom.WriteLine($"0x7074 Action.Attack blocked exploit");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }
                                }
                                break;

                            case ActionType.Pickup: // 0x02
                                {
                                    ActionTarget TargetType = (ActionTarget)packet.ReadUInt8();

                                    byte malformedByte = packet.ReadUInt8();

                                    if (malformedByte != 0x01)
                                    {
                                        try
                                        {
                                            modified.WriteUInt8((byte)CommandType);
                                            modified.WriteUInt8((byte)ActionType);
                                            modified.WriteUInt8((byte)TargetType);
                                            modified.WriteUInt8(0x01);

                                            // 18 09 00 00
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

                                            //Custom.WriteLine("Emulating bot malformed pickup packet");
                                            return response;
                                        }
                                        catch (Exception ex)
                                        {
                                            Custom.WriteLine($"0x7074 Action.Pickup exception {ex.ToString()}");
                                            response.ResultType = PacketResultType.Block;
                                            return response;
                                        }
                                    }
                                }
                                break;


                            /*
                                [C -> S][7074]
                                01                                                ................
                                03                                                ................
                                01                                                ................
                                01                                                ................
                                23 4A 01 00                                       #J..............
                            */
                            case ActionType.Trace: // 0x03
                                {
                                    if(packetLength != 8)
                                    {
                                        Custom.WriteLine($"0x7074 Action.Trace blocked exploit");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }
                                }
                                break;

                            case ActionType.Cast: // 0x04
                                {
                                    try 
                                    {
                                        var RefSkill = Main.Skills;

                                        var Skill_ID = packet.ReadUInt32();
                                        Custom.WriteLine($"ID: {Skill_ID}");

                                        RefSkill.TryGetValue((int)Skill_ID, out var _skill);

                                        if(_skill == null)
                                        {
                                            Custom.WriteLine($"0x7074 Action.Cast sending invalid Skill_ID");
                                            response.ResultType = PacketResultType.Block;
                                            return response;
                                        }

                                        
                                        
                                        /*
                                            Fix exploits here
                                        */

                                        if(packetLength > 12)
                                        {
                                            Custom.WriteLine($"0x7074 Action.Cast packetLength > 12, should never happen");
                                            response.ResultType = PacketResultType.Block;
                                            return response;
                                        }

                                        // RSBot crash emulation
                                        if(packetLength > 7 && packetLength <= 11)
                                        {
                                            modified.WriteUInt8((byte)CommandType);
                                            modified.WriteUInt8((byte)ActionType);
                                            modified.WriteUInt32(Skill_ID);
                                            
                                            packet.ReadUInt8();

                                            modified.WriteUInt8(0x01);
                                            modified.WriteUInt8(0x01);
                                            modified.WriteUInt32(packet.ReadUInt32());

                                            response.ModifiedPackets.Add(new PacketList
                                            {
                                                Packet = modified,
                                                SendImmediately = true,
                                                SecurityType = SecurityType.RemoteSecurity
                                            });

                                            //Custom.WriteLine("Emulating bot malformed skill cast packet");
                                            return response;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //Custom.WriteLine($"0x7074 Action.Cast exception {ex.ToString()}");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }
                                }
                                break;


                            /*
                                [C -> S][7074]
                                01                                                ................
                                05                                                ................
                                29 0C 00 00                                       )...............
                                00                                                ................

                            */
                            case ActionType.Dispel: // 0x05
                                {
                                    if(packetLength != 7)
                                    {
                                        Custom.WriteLine($"0x7074 Action.Dispel blocked exploit");
                                        response.ResultType = PacketResultType.Block;
                                        return response;
                                    }
                                }
                                break;

                            default:
                                response.ResultType = PacketResultType.Block;
                                return response;
                        }
                        break;
                    }

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
                    response.ResultType = PacketResultType.Block;
                    return response;
            }

            return response;
        }
    }
}