using Module.Networking;
using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Server
{
    [Filter(0x3015)]
    public sealed class ServerSpawn : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var response = new PacketHandlingResult();

            return new PacketHandlingResult();

            var h = client.Host()!;
            var character = client.CurrentChar();

            try {
                var uniqId1 = packet.ReadUInt32();
                var uniqId2 = packet.ReadUInt32();
    
                // Happens on spawn?
                if(uniqId2 == character.UniqueID)
                {
                    Custom.WriteLine($"Nigger?");
                    return new PacketHandlingResult();
                }

                Custom.WriteLine($"0x3015 uniqId1: {uniqId1} uniqId2: {uniqId2}");

                switch(character.LastUseItem)
                {
                    /*
                        Attack pets
                    */
                    case 0x08CD: // Item mall
                    case 0x08CC: // Normal
                    {
                        //character.HasAttackPetSummoned = true;
                        //character.AttackPetID = uniqId1;
                    }
                    break;

                    /*
                        Grab pets
                    */
                    case 0x10CD:
                    {
                        //character.HasGrabPetSummoned = true;
                        //character.GrabPetID = uniqId1;
                    }
                    break;
                }

                character.LastUseItem = 0;
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[S->C] 0x3015 {ex}", ConsoleColor.Red);
            }

            return response;
        }
    }
}