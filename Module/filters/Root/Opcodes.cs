namespace NetGuard.Filters.Shared;

// Opcode catalog for the Agent C->S direction (and global framework).
// Source: SilkroadDoc / Agent packets / Global packets.
// Only C->S agent opcodes plus framework opcodes are listed here —
// adding an S->C opcode to this file would let the Agent/Client Landing
// accept a packet the client should never send.

public enum Global : ushort
{
    Identification    = 0x2001,
    KeepAlive         = 0x2002,
    Certification     = 0x6003,
    RelayRequest      = 0x6008,
    RelayResponse     = 0xA008,
    MassiveMessage    = 0x600D,
    Handshake         = 0x5000,
    AcceptHandshake   = 0x9000,
}

public enum Authentication : ushort
{
    Auth = 0x6103,
}

public enum CharScreen : ushort
{
    SelectionJoin   = 0x7001,
    SelectionAction = 0x7007,
    SelectionRename = 0x7450,
}

public enum Exchange : ushort
{
    Start   = 0x7081,
    Confirm = 0x7082,
    Approve = 0x7083,
    Cancel  = 0x7084,
}

public enum Character : ushort
{
    BodyStateRequest = 0x70A7,   // AGENT_CHARACTER_BODYSTATE_REQUEST
    Action           = 0x7074,   // AGENT_PLAYER_ACTION — keep old alias
}

public enum Entity : ushort
{
    Movement = 0x7021,
}

public enum Chat : ushort
{
    Chat = 0x7025,
}

public enum Logout : ushort
{
    Logout = 0x7005,
    Cancel = 0x7006,
}

public enum Inventory : ushort
{
    Operation    = 0x7034,
    StorageOpen  = 0x703C,
    ItemRepair   = 0x703E,
    Unknown703F  = 0x703F,
    ItemUse      = 0x704C,
}

public enum Party : ushort
{
    Create          = 0x7060,
    Leave           = 0x7061,
    Invite          = 0x7062,
    Kick            = 0x7063,
    MatchingForm    = 0x7069,
    MatchingChange  = 0x706A,
    MatchingDelete  = 0x706B,
    MatchingList    = 0x706C,
    MatchingJoin    = 0x706D,
    MatchingPlayerJoin = 0x306E,
}

public enum Academy : ushort
{
    Create            = 0x7470,
    Disband           = 0x7471,
    Unknown7472       = 0x7472,
    Kick              = 0x7473,
    Leave             = 0x7474,
    Grade             = 0x7475,
    Unknown7476       = 0x7476,
    UpdateComment     = 0x7477,
    HonorRank         = 0x7478,
    MatchingRegister  = 0x747A,
    MatchingChange    = 0x747B,
    MatchingDelete    = 0x747C,
    MatchingList      = 0x747D,
    MatchingJoin      = 0x747E,
    MatchingResponse  = 0x347F,
    Unknown7483       = 0x7483,
}

public enum Alchemy : ushort
{
    Reinforce   = 0x7150,
    Enchant     = 0x7151,
    Manufacture = 0x7155,
    Dismantle   = 0x7157,
    Socket      = 0x716A,
}

public enum BattleArena : ushort
{
    Request = 0x74D3,
}

public enum CAS : ushort
{
    Client         = 0x6314,
    ServerResponse = 0x6316,
}

public enum Community : ushort
{
    FriendAdd      = 0x7302,
    FriendResponse = 0x3303,
    FriendDelete   = 0x7304,
    MemoOpen       = 0x7308,
    MemoSend       = 0x7309,
    MemoDelete     = 0x730A,
    MemoList       = 0x730B,
    MemoSendGroup  = 0x730C,
    Block          = 0x730D,
}

public enum Config : ushort
{
    Update = 0x7158,
}

public enum Consignment : ushort
{
    Detail     = 0x7506,
    Close      = 0x7507,
    Register   = 0x7508,
    Unregister = 0x7509,
    Buy        = 0x750A,
    Settle     = 0x750B,
    Search     = 0x750C,
    List       = 0x750E,
}

public enum COS : ushort
{
    Command           = 0x70C5,
    Terminate         = 0x70C6,
    Unknown70C7       = 0x70C7,
    UpdateRidestate   = 0x70CB,
    Unsummon          = 0x7116,
    Name              = 0x7117,
    UpdateSettings    = 0x7420,
}

public enum FGW : ushort
{
    RecallList     = 0x7519,
    RecallMember   = 0x751A,
    RecallResponse = 0x751C,
    Exit           = 0x751D,
}

public enum FlagWar : ushort
{
    Register = 0x74B2,
}

public enum FRPVP : ushort
{
    Update = 0x7516,
}

public enum Game : ushort
{
    Ready1          = 0x3012,
    Ready2          = 0x3014,
    Invite          = 0x3080,
    ResetComplete   = 0x35B6,
}

public enum Guide : ushort
{
    Guide = 0x70EA,
}

public enum Guild : ushort
{
    Create              = 0x70F0,
    Disband             = 0x70F1,
    Leave               = 0x70F2,
    Invite              = 0x70F3,
    Kick                = 0x70F4,
    DonateObsolete      = 0x70F6,
    UpdateNotice        = 0x70F9,
    Promote             = 0x70FA,
    UnionInvite         = 0x70FB,
    UnionLeave          = 0x70FC,
    UnionKick           = 0x70FD,
    UpdateSiegeAuth     = 0x70FF,
    Transfer            = 0x7103,
    UpdatePermission    = 0x7104,
    ElectionStart       = 0x7105,
    ElectionParticipate = 0x7106,
    ElectionVote        = 0x7107,
    WarStart            = 0x7110,
    WarEnd              = 0x7112,
    Unknown7113         = 0x7113,
    WarReward           = 0x7114,
    StorageOpen         = 0x7250,
    StorageClose        = 0x7251,
    StorageList         = 0x7252,
    UpdateNickName      = 0x7256,
    Donate              = 0x7258,
    MercenaryAttr       = 0x7259,
    MercenaryTerminate  = 0x725A,
    GPHistory           = 0x7501,
}

public enum MagicOption : ushort
{
    Grant = 0x34A9,
}

public enum Operator : ushort
{
    Command = 0x7010,
}

public enum Quest : ushort
{
    Talk          = 0x30D4,
    DingDong      = 0x70D8,
    Abandon       = 0x70D9,
    GatherCancel  = 0x70DB,
    RewardSelect  = 0x7515,
}

public enum Siege : ushort
{
    Return = 0x705D,
    Action = 0x705E,
}

public enum Silk : ushort
{
    GachaPlay     = 0x7118,
    GachaExchange = 0x7119,
    History       = 0x711A,
    Unknown7121   = 0x7121,
}

public enum Skill : ushort
{
    Learn             = 0x70A1,
    MasteryLearn      = 0x70A2,
    Withdraw          = 0x7202,
    MasteryWithdraw   = 0x7203,
}

public enum Stall : ushort
{
    Create  = 0x70B1,
    Destroy = 0x70B2,
    Talk    = 0x70B3,
    Buy     = 0x70B4,
    Leave   = 0x70B5,
    Update  = 0x70BA,
}

public enum TAP : ushort
{
    Info   = 0x74DF,
    Update = 0x74E0,
}

public enum Teleport : ushort
{
    Designate = 0x7059,
    Use       = 0x705A,
    Cancel    = 0x705B,
}