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
            public List<Operators> GameMasters { get; set; } = [];
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
