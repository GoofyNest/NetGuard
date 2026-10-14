namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x7034)]
    public sealed class InventoryOperation : IFilter
    {
        /*
            [S -> C][0xB045] (Click / Select vendor)
            byte status = 0x01
            uint32 Vendor UniqueID
            byte unknown = 0x0
            uint32 unknown2 = 05 00 00 00
            byte unknown3 = 0x0

            [C -> S][0x7046] (Buy from vendor)
            uint32 Vendor UniqueID

            [S -> C][0xB046] (Success)
            byte status = 0x01

            ---------------------------------------------------------------

            Selling to vendor

            [C -> S][0x7034] (Client send moving item to vendor)
            byte destination = 0x09
            byte slot = 0x0D
            uint16 = 04 00
            uint32 Vendor UniqueID

            [S -> C][0x304E] (Server giving gold? I guess)
            byte status = 0x01
            uint64 goldAmount
            byte unknown = 0x0

            [S -> C][0xB034] (Server confirm moving item)
            byte status = 0x01
            byte destination = 0x09
            byte slot = 0x0D
            uint16 = 04 00
            uint32 Vendor UniqueID
            byte unknown = 0x05


        */
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            byte destination = packet.ReadUInt8();

            switch(destination)
            {
                case 0x0: // Moving char inventory
                {
                    Custom.WriteLine("Moving item around in player inventory");
                }
                break;

                case 0x09: // Selling to vendor
                {
                    
                }
                break;

                case 0x10: // Moving pet inventory
                {
                    Custom.WriteLine("Moving item around in pet inventory");
                }
                break;

                case 0x1A: // From pet to inventory
                {
                    Custom.WriteLine("From pet to inventory");
                }
                break;

                case 0x1B: // From inventory to pet
                {
                    Custom.WriteLine("From inventory to pet");
                }
                break;

            }

            return response;
        }
    }
}