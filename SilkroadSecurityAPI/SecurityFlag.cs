namespace SilkroadSecurityAPI;

/// <summary>
/// Bit flags embedded in the first byte of a 0x5000 handshake packet.
/// Values match the original Silkroad security flag layout exactly.
/// </summary>
[Flags]
internal enum SecurityFlag : byte
{
    None = 0x00,
    Blowfish = 1 << 1,
    SecurityBytes = 1 << 2,
    Handshake = 1 << 3,
    HandshakeResponse = 1 << 4,
    Reserved6 = 1 << 5,
    Reserved7 = 1 << 6,
    Reserved8 = 1 << 7,
}