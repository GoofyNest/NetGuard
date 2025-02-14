using SilkroadSecurityAPI;

namespace Module.Engine.Classes
{
    public class SessionData
    {
        public PlayerInformation playerInfo { get; set; } = new();
        public GatewayServer gatewaySettings { get; set; } = new();
        public GameServer gameSettings { get; set; } = new();
        public ShardServer shardSettings { get; set; } = new();
    }

    public class PlayerInformation
    {
        public string currentChar { get; set; } = string.Empty;
        public bool sentJoinRequest = false;
        public IPInformation ipInfo { get; set; } = null!;
        public AccountInformation accInfo { get; set; } = null!;
        public List<CharacterInformation> charInfo { get; set; } = null!;
    }

    public class IPInformation
    {
        public string ip { get; set; } = string.Empty;
        public DateTime timestamp { get; set; } = DateTime.UtcNow;
    }

    public class AccountInformation
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string mac { get; set; } = string.Empty;
        public DateTime timestamp { get; set; } = DateTime.UtcNow;
    }

    public class CharacterInformation
    {
        public string charname { get; set; } = string.Empty;

        public long serverTime { get; set; }
        public long refObjID { get; set; }
        public int scale { get; set; }
        public int curLevel { get; set; }
        public int maxLevel { get; set; }
        public ulong expOffset { get; set; }
        public long sExpOffset { get; set; }
        public ulong remainGold { get; set; }
        public long remainSkillPoint { get; set; }
        public int remainStatPoint { get; set; }
        public int remainHwanCount { get; set; }
        public long gatheredExpPoint { get; set; }
        public long hp { get; set; }
        public long mp { get; set; }
        public int autoInverstExp { get; set; }
        public int dailyPk { get; set; }
        public int totalPk { get; set; }
        public long pkPenaltyPoint { get; set; }
        public int hwanLevel { get; set; }
        public int pvpCape { get; set; }

        public CharacterInventory inventory { get; set; } = null!;

        public DateTime timestamp { get; set; } = DateTime.UtcNow;
    }

    public class CharacterInventory
    {
        public int size { get; set; }
        public int itemCount { get; set; }

        public List<Item> items { get; set; } = null!;
    }

    public class Item
    {
        public int slot { get; set; }
        public long id { get; set; }
        public string codeName128 { get; set; } = string.Empty;
        public int optLevel { get; set; }
        public ulong variance { get; set; }
        public long durability { get; set; }
        public int magParamNum { get; set; }

        public override string ToString()
        {
            return $"Slot: {slot}, ID: {id}, CodeName128: {codeName128}, OptLevel: {optLevel}, Variance: {variance}, Durability: {durability}, MagParamNum: {magParamNum}";
        }
    }

    public class GatewayServer
    {
        public ushort serverID { get; set; }

        public bool sentPatchResponse { get; set; } = false;
        public bool sentShardListResponse { get; set; } = false;
    }

    public class GameServer
    {
        public Packet charData { get; set; } = null!;
        public int charDataProcess { get; set; } = 0;
    }

    public class ShardServer
    {
        public bool inCharSelection { get; set; } = false;
        public bool exploitIwaFix { get; set; } = true;
    }
}
