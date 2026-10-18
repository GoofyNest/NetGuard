using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB04C)]
    public sealed class PlayerUseItem : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            byte status = packet.ReadUInt8();

            if(status == 0x01)
            {
                byte Slot = packet.ReadUInt8();
                ushort unk = packet.ReadUInt16();
                ushort ItemType = packet.ReadUInt16();

                switch(ItemType)
                {
                    /*
                        Rideable horse
                        Trade pets
                    */
                    case 0x11ED: // Item mall
                    case 0x11EC: // Normal
                    {
                        h.UseItem.HorseTime = DateTime.UtcNow;
                    }
                    break;

                    /*
                        Grab pets
                    */
                    case 0x10CD:
                    {
                        h.UseItem.GrabPetTime = DateTime.UtcNow;
                    }
                    break;

                    /*
                        Attack pets
                    */
                    case 0x08CD: // Item mall
                    case 0x08CC: // Normal
                    {
                        h.UseItem.AttackPetTime = DateTime.UtcNow;
                    }
                    break;
                }

            }

            return response;
        }
    }
}