namespace GatewayModule.Engine.Classes
{
    public class SessionData
    {
        public string ip { get; set; }
        public string StrUserID { get; set; } = "";
        public string password { get; set; } = "";

        public ushort serverID { get; set; }

        public int sent_id { get; set; } = 0;
        public int sent_list { get; set; } = 0;
    }
}
