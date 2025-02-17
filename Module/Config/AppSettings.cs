using Newtonsoft.Json;

namespace Module.Config
{
    public enum FileVersion
    {
        jSRO,
        iSRO,
        vSRO,
        cSRO
    }

    public class AppSettings
    {
        public bool DisableLogWriting { get; set; } = true;
        public bool DisableConsoleLogging { get; set; } = false;
        public AgentSettings Agent { get; set; } = new();
        public Version ServerType { get; set; } = new();
        public ClientData Data { get; set; } = new();

        public class ClientData()
        {
            public string Path { get; set; } = "client_data";
        }

        public class AgentSettings()
        {
            public OperatorConfiguration GameMasterConfig { get; set; } = new();
        }

        public class OperatorConfiguration()
        {
            public string Description { get; set; } = "Use our Advanced permission system for controlling your [GM]'s";
            public bool EnablePermissionSystem { get; set; } = false;
            public List<Operators> GMs { get; set; } = [];
            public OperatorMisc Misc { get; set; } = new();
        }

        public class OperatorMisc()
        {
            public string Warning { get; set; } = "Experimental system idea!!! This system is to make every player a [GM] so the filter can use advanced commands like teleport player, spawn item.";
            public string Warning2 { get; set; } = "This system only supports jSRO for now, make sure sec_content and sec_primary is set to 1 for all users";
            public bool DisableGMChat { get; set; } = false;
            public bool DisableGMConsole { get; set; } = false;
            public bool SpawnVisible { get; set; } = false;
        }

        public class Operators()
        {
            public string Username { get; set; } = string.Empty;
            public bool ShouldSpawnVisible { get; set; } = false;
            public bool NoPinkChat { get; set; } = false;
            public List<string> Permissions { get; set; } = [];
            [JsonIgnore]
            public List<string> DefaultPermissions { get; set; } =
            [
                "FindUser",
                "GoTown",
                "ToTown",
                "WorldStatus",
                "LoadMonster",
                "MakeItem",
                "MoveToUser",
                "ZoeMonster",
                "KickPlayer",
                "Invisible",
                "Invincible",
                "Warp",
                "RecallUser",
                "AllowLogout",
                "InitQ",
                "ResetQ",
                "CompQ",
                "RemoveQ",
                "MoveToNpc",
                "StartCTF",
                "SendNotice"
            ];
        }

        public class Version
        {
            public string Description { get; set; } = "Defines the server version type";
            public string[] AllowedValues { get; set; } = Enum.GetNames(typeof(FileVersion));
            public string CurrentValue { get; set; } = FileVersion.jSRO.ToString();
        }

        public GatewaySettings Gateway { get; set; } = new();

        public class GatewaySettings()
        {
            public Captcha LoginCaptcha { get; set; } = new();
        }

        public class Captcha()
        {
            public bool DisableCaptcha { get; set; } = false;
            public string StaticCaptchaCode { get; set; } = "b";
        }
    }
}
