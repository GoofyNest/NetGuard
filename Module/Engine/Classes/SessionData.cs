using SilkroadSecurityAPI;

namespace Module.Engine.Classes
{
    public class SessionData
    {
        public string ip { get; set; } = string.Empty;
        public string StrUserID { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string mac { get; set; } = string.Empty;

        public ushort serverID { get; set; }

        public int sent_id { get; set; } = 0;
        public int sent_list { get; set; } = 0;

        // Chardata packet
        public Packet charData { get; set; } = null!;
        public int charDataProcess { get; set; } = 0;

        // Exploit fixes
        public bool inCharSelection { get; set; } = false;
        public bool exploitIwaFix { get; set; } = true;


    }
}
