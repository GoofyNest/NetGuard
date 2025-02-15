using System.Collections.Generic;

namespace NetGuardLoader.Classes
{
    public enum ModuleType : byte
    {
        GatewayModule,
        DownloadModule,
        AgentModule
    }

    public class Config
    {
        public List<ModuleSettings> ModuleBinding = new();
        public string DllPath { get; set; } = "Module.dll";

        public string LogFolder = "log";

        public string BindingsPath { get; set; } = "config\\bindings.json";
        public string SettingsPath { get; set; } = "config\\settings.json";
        public string LogFile { get; set; } = string.Empty;
    }

    public class ModuleSettings
    {
        public string Name { get; set; } = "Example #1";
        public string GuardIP { get; set; } = "127.0.0.1";
        public int GuardPort { get; set; } = 15779;
        public string ModuleIP { get; set; } = "127.0.0.1";
        public int ModulePort { get; set; } = 5779;
        public ModuleType ModuleType { get; set; } = 0;
    }
}