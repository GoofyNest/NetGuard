using Module.Services;

namespace Module.Helpers.PacketManager.Agent.Client
{
    public static class Packets
    {
        public static readonly HashSet<ushort> BadOpcodes =
        [
            0x0000, // unk1
            0x3510, // GameServerCrasher
            0x34BB, // SpawnMonster
            0x2005, // T46TOOL
            0x1005, // SRFUCKER
            0x7777, // AgentCrasher
            0x631D, // SampleLag
            0x0000, // Unknown
            0x200a, // SRDOS4
            0xa003, // AgentBlock
            0x1001, // unk2
            0x1002, // unk3
            0x1003, // unk4
            0x1004, // unk5
            0x1006, // unk6
            0x1007, // unk7
            0x2311, // unk8
            0x2206, // unk9
            0x2209, // unk10
            0x220a, // unk11
            0x220e, // unk12
            0x220f, // unk13
            0x6003, // unk14
            0x6005, // unk15
            0x6006, // unk16
            0x6105, // unk17
            0x620D, // unk18
            0x6300, // unk19
            0x6110, // unk20
            0x6207, // unk21
            0x6308, // unk22
            0x6312, // unk23
            0x6303, // unk24
            0x6905, // unk25
            0x6912, // unk26
            0x6315, // unk27
            0x6902, // unk28
            0xa006, // unk29
            0xa008, // unk30
            0xa306, // unk31
            0xa208, // unk32
            0xa200, // unk33
            0xa203, // unk34
            0x4444  // unk35
        ];

        public static List<ushort> GoodOpcodes { get; private set; } = [];
        public static void Init()
        {
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Global)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Academy)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Alchemy)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Authentication)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(BattleArena)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(CAS)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Character)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(CharScreen)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Chat)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Community)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Config)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Consignment)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(COS)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Entity)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Exchange)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(FGW)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(FlagWar)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(FRPVP)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Game)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Guide)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Guild)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Inventory)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Logout)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(MagicOption)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Operator)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Party)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Quest)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Siege)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Silk)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Skill)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Stall)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(TAP)).Cast<ushort>());
            GoodOpcodes.AddRange(Enum.GetValues(typeof(Teleport)).Cast<ushort>());

            Custom.WriteLine($"GoodOpcodes count: {GoodOpcodes.Count}");
        }
    }

    public enum Global : ushort
    {
        Identification = 0x2001,
        Ping = 0x2002,
        HandShake = 0x5000,
        AcceptHandshake = 0x9000,
    }

    public enum Academy : ushort
    {
        Create = 0x7470,
        Disband = 0x7471,
        unk = 0x7472,
        Kick = 0x7473,
        Leave = 0x7474,
        Grade = 0x7475,
        unk2 = 0x7476,
        UpdateComment = 0x7477,
        HonorRank = 0x7478,
        MatchingRegister = 0x747A,
        MatchingChange = 0x747B,
        MatchingDelete = 0x747C,
        MatchingList = 0x747D,
        MatchingJoin = 0x747E,
        MatchingResponse = 0x347F,
        unk3 = 0x7483,
    }

    public enum Alchemy : ushort
    {
        Reinforce = 0x7150,
        Enchant = 0x7151,
        Manufacture = 0x7155,
        Dismantle = 0x7157,
        Socket = 0x716A
    }

    public enum Authentication : ushort
    {
        Auth = 0x6103,
    }

    public enum BattleArena : ushort
    {
        Request = 0x74D3
    }

    public enum CAS : ushort // Customer Advisory Service
    {
        Client = 0x6314,
        ServerResponse = 0x6316
    }

    public enum Character : ushort
    {
        InfoUpdate = 0x70A7,
        Action = 0x7074,
    }

    public enum CharScreen : ushort
    {
        SelectionJoin = 0x7001,
        SelectionAction = 0x7007,
        SelectionRename = 0x7450
    }

    public enum Chat : ushort
    {
        Chat = 0x7025
    }

    public enum Community : ushort
    {
        FriendAdd = 0x7302,
        FriendResponse = 0x3303,
        FriendDelete = 0x7304,
        MemoOpen = 0x7308,
        MemoSend = 0x7309,
        MemoDelete = 0x730A,
        MemoList = 0x730B,
        MemoSendGroup = 0x730C,
        Block = 0x730D
    }

    public enum Config : ushort
    {
        Update = 0x7158
    }

    public enum Consignment : ushort
    {
        Detail = 0x7506,
        Close = 0x7507,
        Register = 0x7508,
        Unregister = 0x7509,
        Buy = 0x750A,
        Settle = 0x750B,
        Search = 0x750C,
        List = 0x750E
    }

    public enum COS : ushort // Call On Summons
    {
        Command = 0x70C5,
        Terminate = 0x70C6,
        unk = 0x70C7,
        UpdateRidestate = 0x70CB,
        Unsummon = 0x7116,
        Name = 0x7117,
        UpdateSettings = 0x7420
    }

    public enum Entity : ushort
    {
        EntityMovement = 0x7021
    }

    public enum Exchange : ushort
    {
        Start = 0x7081,
        Confirm = 0x7082,
        Approve = 0x7083,
        Cancel = 0x7084
    }

    public enum FGW : ushort // Forgotten World
    {
        RecallList = 0x7519,
        RecallMember = 0x751A,
        RecallResponse = 0x751C,
        Exit = 0x751D
    }

    public enum FlagWar : ushort // Capture the Flag
    {
        Register = 0x74B2
    }

    public enum FRPVP : ushort // Free PVP
    {
        Update = 0x7516
    }

    public enum Game : ushort
    {
        Ready1 = 0x3012,
        Ready2 = 0x3014, // Happens if locale is different in client
        Invite = 0x3080,
        ResetComplete = 0x35B6
    }

    public enum Guide : ushort
    {
        Guide = 0x70EA,
    }

    public enum Guild : ushort
    {
        Create = 0x70F0,
        Disband = 0x70F1,
        Leave = 0x70F2,
        Invite = 0x70F3,
        Kick = 0x70F4,
        DonateObsolete = 0x70F6,
        UpdateNotice = 0x70F9,
        Promote = 0x70FA,
        UnionInvite = 0x70FB,
        UnionLeave = 0x70FC,
        UnionKick = 0x70FD,
        UpdateSiegeAuth = 0x70FF,
        Transfer = 0x7103,
        UpdatePermission = 0x7104,
        ElectionStart = 0x7105,
        ElectionParticipate = 0x7106,
        ElectionVote = 0x7107,
        WarStart = 0x7110,
        WarEnd = 0x7112,
        unk = 0x7113,
        WarReward = 0x7114,
        StorageOpen = 0x7250,
        StorageClose = 0x7251,
        StorageList = 0x7252,
        UpdateNickName = 0x7256,
        Donate = 0x7258,
        MercenaryAttr = 0x7259,
        MercenaryTerminate = 0x725A,
        GPHistory = 0x7501
    }

    public enum Inventory : ushort
    {
        Operation = 0x7034,
        StorageOpen = 0x703C,
        ItemRepair = 0x703E,
        unk = 0x703F,
        ItemUse = 0x704C
    }

    public enum Logout : ushort
    {
        Logout = 0x7005,
        Cancel = 0x7006
    }

    public enum MagicOption : ushort
    {
        Grant = 0x34A9
    }

    public enum Operator : ushort
    {
        Command = 0x7010
    }

    public enum Party : ushort
    {
        Create = 0x7060,
        Leave = 0x7061,
        Invite = 0x7062,
        Kick = 0x7063,
        MatchingForm = 0x7069,
        MatchingChange = 0x706A,
        MatchingDelete = 0x706B,
        MatchingList = 0x706C,
        MatchingJoin = 0x706D,
        MatchinPlayerJoin = 0x306E
    }

    public enum Quest : ushort
    {
        Talk = 0x30D4,
        DingDong = 0x70D8,
        Abandon = 0x70D9,
        GatherCancel = 0x70DB,
        RewardSelect = 0x7515
    }

    public enum Siege : ushort
    {
        Return = 0x705D,
        Action = 0x705E
    }

    public enum Silk : ushort
    {
        GachaPlay = 0x7118,
        GachaExhange = 0x7119,
        History = 0x711A,
        unk = 0x7121,
    }

    public enum Skill : ushort
    {
        Learn = 0x70A1,
        MasteryLearn = 0x70A2,
        Withdraw = 0x7202,
        MasteryWithdraw = 0x7203
    }

    public enum Stall : ushort
    {
        Create = 0x70B1,
        Destroy = 0x70B2,
        Talk = 0x70B3,
        Buy = 0x70B4,
        Leave = 0x70B5,
        Update = 0x70BA,
    }

    public enum TAP : ushort // Temple Area Points
    {
        Info = 0x74DF,
        Update = 0x74E0
    }

    public enum Teleport : ushort
    {
        Designate = 0x7059,
        Use = 0x705A,
        Cancel = 0x705B
    }
}
