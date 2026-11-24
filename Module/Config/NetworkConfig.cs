namespace Module.Config
{
    public enum ModuleType : byte
    {
        GatewayModule,
        DownloadModule,
        AgentModule
    }

    public class NetworkConfig
    {
        public List<ModuleSettings> ModuleBinding = [];
        public string DllPath { get; set; } = "Module.dll";
        public string LogFolder = "log";
        public string FiltersPath { get; set; } = "filters";
        public string BindingsPath { get; set; } = "config\\NetworkConfig.json";
        public string SettingsPath { get; set; } = "config\\AppSettings.json";
        public string LogFile { get; set; } = string.Empty;
    }

    public class ModuleSettings
    {
        public int MaxConnections { get; set; } = 5000;
        public int MaxConnectionsPerIP { get; set; } = 100;
        public int HandshakeTimeoutSeconds { get; set; } = 30;
        public int LoginTimeoutSeconds { get; set; } = 120;
        public int DrainTimeoutSeconds { get; set; } = 10;
        internal void ValidateLimits()
        {
            if (MaxConnections < 1 || MaxConnectionsPerIP < 1 || HandshakeTimeoutSeconds < 1
                || LoginTimeoutSeconds < HandshakeTimeoutSeconds || DrainTimeoutSeconds < 1
                || LoginTimeoutSeconds > 86400 || DrainTimeoutSeconds > 300)
                throw new InvalidOperationException("Invalid connection limits or network deadlines.");
        }
        public string Name { get; set; } = "Example #1";
        public string GuardIP { get; set; } = "127.0.0.1";
        public int GuardPort { get; set; } = 15779;
        public string ModuleIP { get; set; } = "127.0.0.1";
        public int ModulePort { get; set; } = 5779;
        public ModuleType ModuleType { get; set; } = 0;
        public string ApiHost { get; set; } = "127.0.0.1";
        public string ApiListenIP { get; set; } = "127.0.0.1";
        public int ApiPort { get; set; } = 0;
        public string ApiPassword { get; set; } = "";
    }
}
