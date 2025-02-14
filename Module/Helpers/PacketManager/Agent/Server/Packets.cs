namespace Module.Helpers.PacketManager.Agent.Server
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