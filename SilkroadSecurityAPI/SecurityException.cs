namespace SilkroadSecurityAPI;

/// <summary>
/// Raised when the security layer detects an invalid packet, a bad handshake,
/// a CRC/count-byte mismatch, or any other protocol-level violation.
/// </summary>
public sealed class SecurityException : Exception
{
    public SecurityException(string message) : base(message) { }
    public SecurityException(string message, Exception inner) : base(message, inner) { }
}