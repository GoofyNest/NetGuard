namespace Module.Classes
{
    public class Settings
    {
        public bool logWriting { get; set; } = true;
        public bool disableConsole { get; set; } = false;
        public AgentSettings agentSettings { get; set; } = new();
        public ClientDataSettings clientDataSettings { get; set; } = new();

        public ServerVersionInfo serverVersion { get; set; } = new();

        public class ClientDataSettings()
        {
            public string path { get; set; } = "client_data";
        }

        public class AgentSettings()
        {
            
        }

        public class ServerVersion
        {
            public enum Version
            {
                jSRO,
                iSRO,
                vSRO,
                cSRO
            }
        }

        public class ServerVersionInfo
        {
            public string Description { get; set; } = "Defines the server version type";
            public string[] AllowedValues { get; set; } = Enum.GetNames(typeof(ServerVersion.Version));
            public string CurrentValue { get; set; } = ServerVersion.Version.jSRO.ToString();
        }

        public GatewaySettings gatewaySettings { get; set; } = new();

        public class GatewaySettings()
        {
            public IBuvChallange captchaSettings { get; set; } = new();
        }

        public class IBuvChallange()
        {
            public bool bypassCaptcha { get; set; } = false;
            public string captchaCode { get; set; } = "b";
        }
    }
}
