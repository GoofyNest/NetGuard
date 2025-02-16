using SilkroadSecurityAPI;

namespace Module.Engine.Classes
{
    public class SessionData
    {
        public PlayerInformation PlayerInfo { get; set; } = null!;
        public GatewayServer Gateway { get; set; } = null!;
        public AgentServer Agent { get; set; } = null!;
    }

    public class PlayerInformation
    {
        public string CurrentCharName { get; set; } = string.Empty;
        public IPInformation IpInfo { get; set; } = null!;
        public AccountInformation AccInfo { get; set; } = null!;
        public List<CharacterInformation> CharInfo { get; set; } = null!;
    }

    public class IPInformation
    {
        public string Ip { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class AccountInformation
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Mac { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class CharacterInformation
    {
        public string Charname { get; set; } = string.Empty;
        public bool isVisible { get; set; } = false;

        public long ServerTime { get; set; }
        public long RefObjID { get; set; }
        public int Scale { get; set; }
        public int CurLevel { get; set; }
        public int MaxLevel { get; set; }
        public ulong ExpOffset { get; set; }
        public long SExpOffset { get; set; }
        public ulong RemainGold { get; set; }
        public long RemainSkillPoint { get; set; }
        public int RemainStatPoint { get; set; }
        public int RemainHwanCount { get; set; }
        public long GatheredExpPoint { get; set; }
        public long Hp { get; set; }
        public long Mp { get; set; }
        public int AutoInverstExp { get; set; }
        public int DailyPk { get; set; }
        public int TotalPk { get; set; }
        public long PkPenaltyPoint { get; set; }
        public int HwanLevel { get; set; }
        public int PvpCape { get; set; }

        public CharacterInventory Inventory { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class CharacterInventory
    {
        public int Size { get; set; }
        public int ItemCount { get; set; }

        public List<Item> Items { get; set; } = null!;
    }

    public class Item
    {
        public int Slot { get; set; }
        public long Id { get; set; }
        public string CodeName128 { get; set; } = string.Empty;
        public int OptLevel { get; set; }
        public ulong Variance { get; set; }
        public long Durability { get; set; }
        public int MagParamNum { get; set; }

        public override string ToString()
        {
            return $"Slot: {Slot}, ID: {Id}, CodeName128: {CodeName128}, OptLevel: {OptLevel}, Variance: {Variance}, Durability: {Durability}, MagParamNum: {MagParamNum}";
        }
    }

    public class GatewayServer
    {
        public ushort ServerID { get; set; }

        public bool SentPatchResponse { get; set; } = false;
        public bool SentShardListResponse { get; set; } = false;
    }

    public class AgentServer
    {
        public bool InCharSelectionScreen { get; set; } = false;
        public bool SentJoinRequest {  get; set; } = false;
        public bool IsIngame { get; set; } = false;
        public Packet CharData { get; set; } = null!;
        public int CharDataProcess { get; set; } = 0;
    }
}
