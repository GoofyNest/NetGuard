namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7074)]
    public sealed class CharAction : IFilter
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
                        case ActionType.Pickup:
                        {
                            ActionTarget TargetType = (ActionTarget)packet.ReadUInt8();

                            byte malformedByte = packet.ReadUInt8();

                            if (malformedByte > 2)
                            {
                                try
                                {
                                    modified.WriteUInt8((byte)CommandType);
                                    modified.WriteUInt8((byte)ActionType);
                                    modified.WriteUInt8((byte)TargetType);
                                    modified.WriteUInt8(0x01);

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
                                    return response;
                                }
                            }
                            break;
                        }
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
                    return response;
            }

            return response;
        }
    }
}