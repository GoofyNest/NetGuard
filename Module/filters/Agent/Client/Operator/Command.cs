using NetGuard.Hot;

namespace NetGuard.Filters.Agent.Client
{
    public enum Action : int
    {
        FindUser = 1, GoTown = 2, ToTown = 3, WorldStatus = 4,
        LoadMonster = 6, MakeItem = 7, MoveToUser = 8, ZoeMonster = 12,
        KickPlayer = 13, Invisible = 14, Invincible = 15, Warp = 16,
        RecallUser = 17, AllowLogout = 24, InitQ = 27, ResetQ = 28,
        CompQ = 29, RemoveQ = 30, MoveToNpc = 31, StartCTF = 48,
        SendNotice = 91923,
    }

    [Filter(0x7010)]
    public sealed class GMCommand : IFilter
    {
        public PacketHandlingResult Handle(Packet packet, SessionData client, SessionContext ctx)
        {
            var h = client.Host()!;
            var response = new PacketHandlingResult();

            var Settings = Main.Settings.Agent;
            var username = client.PlayerInfo.Username;
            var adminIndex = Settings.GameMasterConfig.GMs.FindIndex(m => m.Username == username);

            if (adminIndex == -1 && Settings.GameMasterConfig.Misc.DisableGMConsole)
            {
                response.ResultType = PacketResultType.Block;
                return response;
            }

            if (!Settings.GameMasterConfig.EnablePermissionSystem)
                return response;

            if (adminIndex == -1)
            {
                response.ResultType = PacketResultType.Block;
                Custom.WriteLine($"Blocked GM command from: {username}");
                return response;
            }

            var _permissions = Settings.GameMasterConfig.GMs[adminIndex].Permissions;

            int action = packet.ReadUInt8();
            if (Enum.IsDefined(typeof(Action), action))
            {
                var enumName = ((Action)action).ToString().Trim();
                if (!_permissions.Contains(enumName))
                {
                    Custom.WriteLine($"GM Commands: {username} Missing permission: {action}:{enumName}");
                    response.ResultType = PacketResultType.Block;
                    return response;
                }
            }
            else
            {
                Custom.WriteLine($"Invalid GM command: {action}");
                response.ResultType = PacketResultType.Block;
            }

            return response;
        }
    }
}