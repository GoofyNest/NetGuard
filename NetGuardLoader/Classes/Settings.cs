namespace NetGuardLoader.Classes
{
    public class Settings
    {
        public AgentSettings agentSettings { get; set; } = new();

        public class AgentSettings()
        {

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
