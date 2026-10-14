namespace NetGuard.Filters.Agent.Client
{
    [Filter(0x70A2)]
    public sealed class SkillMasteryLearn : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client)
        {
            var response = new PacketHandlingResult();

            packet.ReadUInt32(); // masteryid

            var level = packet.ReadUInt8();

            if (level > 1)
            {
                Custom.WriteLine($"Prevented {client.PlayerInfo.AccInfo.Username}, using MasteryLevel exploit", ConsoleColor.Yellow);
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}