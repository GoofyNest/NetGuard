namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7021)]
    public sealed class PlayerWalk : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            

            //byte action = packet.ReadUInt8();
            //if (action == 2) // Auto Potion
            //{
            //    byte hp_unk  = packet.ReadUInt8();
            //    byte hp_unk2 = packet.ReadUInt8();
            //    byte mp_unk  = packet.ReadUInt8();
            //    byte mp_unk2 = packet.ReadUInt8();
            //    byte unk5    = packet.ReadUInt8();
            //    byte unk6    = packet.ReadUInt8();
            //    byte delay   = packet.ReadUInt8();
            //}

            return response;
        }
    }
}