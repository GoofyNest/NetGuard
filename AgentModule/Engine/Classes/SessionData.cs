using SilkroadSecurityAPI;

namespace NetGuard.Engine.Classes
{
    public class SessionData
    {
        public string ip { get; set; }
        public string StrUserID { get; set; } = "";
        public string password { get; set; } = "";
        public string mac { get; set; }

        // Chardata packet
        public Packet charData { get; set; } = null;
        public int charDataProcess { get; set; } = 0;

        // Exploit fixes
        public bool inCharSelection { get; set; } = false;
        public bool exploitIwaFix { get; set; } = true;

    }
}
