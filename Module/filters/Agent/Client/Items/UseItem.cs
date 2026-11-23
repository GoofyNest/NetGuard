using NetGuard.Hot;
using NetGuard.Hot.Helpers;

namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x704C)]
    public sealed class UseItem : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            if (packet.RemainingRead() < 3)
                return new PacketHandlingResult { ResultType = PacketResultType.Block };
            packet.ReadUInt8(); // Slot
            ushort useType = packet.ReadUInt16();
            int type = PetTracking.ItemType(useType);
            var character = client.CurrentChar();

            if (character == null)
                return new PacketHandlingResult { ResultType = type != 0 ? PacketResultType.Block : PacketResultType.None };

            if (!PetTracking.TryUse(character, type))
            {
                ctx?.SendToClient(ChatPackets.CreateNotice(
                    !character.PetStateReady
                        ? "Pet data is not ready. Teleport or relog before summoning."
                        : "Only one attack pet and one grab pet may be summoned. Wait for the current pet action to finish."));
                return new PacketHandlingResult { ResultType = PacketResultType.Block };
            }
            return new PacketHandlingResult();
        }
    }
}
