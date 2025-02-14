namespace Module.Helpers.PacketManager.Agent
{
    public static class ClientPackets
    {
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
            ClientPlayerBerserk = 0x70A7,
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