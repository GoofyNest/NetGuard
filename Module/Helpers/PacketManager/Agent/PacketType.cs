namespace Module.Helpers.PacketManager.Agent
{
    public static class ClientPackets
    {
        public static readonly HashSet<int> badOpcodes = new HashSet<int>
        {
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
        };

        public enum Global : ushort
        {
            Identification = 0x2001,
            Ping = 0x2002,
            HandShake = 0x5000,
            AcceptHandshake = 0x9000,
        }

        public enum Login : ushort
        {
            LogoutRequest = 0x7005,
            AuthRequest = 0x6103,  // Response: SERVER_AGENT_AUTH_RESPONSE
        }

        public enum Shard : ushort
        {
            CharacterSelectionRenameRequest = 0x7450,
            CharacterSelectionJoinRequest = 0x7001,
            CharacterSelectionActionRequest = 0x7007,  // Response: SERVER_AGENT_CHARACTER_SELECTION_RESPONSE
        }

        public enum Agent : ushort
        {
            ConfigUpdate = 0x7158,
            PlayerBerserk = 0x70A7,
            SkillMasteryLevel = 0x70A2,
            MagicOptionGrant = 0x34A9,
            GuildUpdateNotice = 0x70F9,
            FortressSiegeAction = 0x705E,
            ActionRequest = 0x7074,
            GameReady = 0x3012,
            GameReady2 = 0x3014,  // Happens if locale is different in client
        }
    }

    public static class ServerPackets
    {
        public enum Global : ushort
        {
            Identification = 0x2001,
            Ping = 0x2002,
            HandShake = 0x5000,
            NodeStatus1 = 0x2005,
            NodeStatus2 = 0x6005,
        }

        public enum Login : ushort
        {
            AuthResponse = 0xA103, // IPLimit = 5, ServerIsFull = 4
            CharacterSelectionResponse = 0xB007,
        }

        public enum Agent : ushort
        {
            CharacterDataBegin = 0x34A5,
            CharacterData = 0x3013,
            CharacterDataEnd = 0x34A6,
            CharacterJoin = 0xB001,
            COSInfo = 0x30C8,
            COSUpdate = 0x30C9,
            COSUpdateRideState = 0xB0CB,
            EnvironmentCelestialPosition = 0x3020
        }
    }
}