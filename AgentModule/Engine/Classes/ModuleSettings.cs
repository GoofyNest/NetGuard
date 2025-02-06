using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentModule.Engine.Classes
{
    public class ModuleSettings
    {
        public string guardIP { get; set; } = "127.0.0.1";
        public int guardPort { get; set; } = 15779;
        public string moduleIP { get; set; } = "127.0.0.1";
        public int modulePort { get; set; } = 5779;
    }
}
