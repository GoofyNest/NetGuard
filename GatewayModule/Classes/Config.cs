using System.Collections.Generic;

namespace NetGuard.Classes
{
    public class Config
    {
        public List<GatewaySettings> _gatewayModules = new List<GatewaySettings>();
        public List<AgentSettings> _agentModules = new List<AgentSettings>();
    }

    public class GatewaySettings
    {
        public string name { get; set; } = "Example #1";
        public string guardIP { get; set; } = "127.0.0.1";
        public int guardPort { get; set; } = 15779;
        public string moduleIP { get; set; } = "127.0.0.1";
        public int modulePort { get; set; } = 5779;
    }

    public class AgentSettings
    {
        public string name { get; set; } = "Example #1";
        public string guardIP { get; set; } = "127.0.0.1";
        public int guardPort { get; set; } = 15884;
        public string moduleIP { get; set; } = "127.0.0.1";
        public int modulePort { get; set; } = 5884;
    }
}
