using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0xB04C)]
    public sealed class PlayerUseItem : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            try {
                var h = client.Host()!;
                byte status = packet.ReadUInt8();
                var character = client.CurrentChar();
                if (character != null)
                    PetTracking.UseResult(character, status == 0x01);

                if(status == 0x01)
                {
                    byte Slot = packet.ReadUInt8();
                    ushort unk = packet.ReadUInt16();
                    ushort ItemType = packet.ReadUInt16();

                    if (character != null) character.LastUseItem = ItemType;

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
                            //character.HasGrabPetSummoned = true;
                            h.UseItem.GrabPetTime = DateTime.UtcNow;
                        }
                        break;

                        /*
                            Attack pets
                        */
                        case 0x08CD: // Item mall
                        case 0x08CC: // Normal
                        {
                            //character.HasAttackPetSummoned = true;
                            h.UseItem.AttackPetTime = DateTime.UtcNow;
                        }
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[S->C] 0xB04C {ex}", ConsoleColor.Red);
            }

            return response;
        }
    }
}
