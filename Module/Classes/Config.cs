﻿using System.Collections.Generic;

namespace Module.Classes
{
    public enum ModuleType : byte
    {
        GatewayModule,
        DownloadModule,
        AgentModule
    }

    public class Config
    {
        public List<ModuleSettings> ModuleBinding = new List<ModuleSettings>();
        public string dllPath { get; set; } = "Module.dll";
    }

    public class ModuleSettings
    {
        public string name { get; set; } = "Example #1";
        public string guardIP { get; set; } = "127.0.0.1";
        public int guardPort { get; set; } = 15779;
        public string moduleIP { get; set; } = "127.0.0.1";
        public int modulePort { get; set; } = 5779;
        public ModuleType moduleType { get; set; } = 0;
    }
}
