using Module.Networking;
using SilkroadSecurityAPI;

namespace Module.PacketHandler.Filtering
{
    /// <summary>
    /// Exactly one Landing may exist per FilterContext. Runs for every packet
    /// in that context, before opcode dispatch. Return null to pass through,
    /// or a PacketHandlingResult to short-circuit.
    /// </summary>
    public interface ILanding
    {
        PacketHandlingResult? Handle(Packet packet, SessionData client, SessionContext ctx);
    }
}