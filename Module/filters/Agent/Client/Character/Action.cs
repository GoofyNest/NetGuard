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
            var response = new PacketHandlingResult();

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

                        switch (ActionType)
                        {
                            case ActionType.Attack:
                                {

                                }
                                break;

                            case ActionType.Trace:
                                {

                                }
                                break;

                            case ActionType.Cast:
                                {

                                }
                                break;

                            case ActionType.Dispel:
                                {

                                }
                                break;

                            case ActionType.Pickup:
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
                                            Custom.WriteLine($"Error emulating pickup {ex.ToString()}");
                                            response.ResultType = PacketResultType.Block;
                                            return response;
                                        }
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