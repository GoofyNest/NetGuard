namespace SilkroadSecurityAPI;

/// <summary>
/// Legacy slice descriptor retained for API compatibility.
/// New code should prefer <see cref="Memory{T}"/> / <see cref="Span{T}"/>.
/// </summary>
public sealed class TransferBuffer
{
    public byte[] Buffer { get; set; }
    public int Offset { get; set; }
    public int Size { get; set; }

    public TransferBuffer()
    {
        Buffer = [];
    }

    public TransferBuffer(int length) : this(length, 0, 0) { }

    public TransferBuffer(int length, int offset, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Buffer = length == 0 ? [] : new byte[length];
        Offset = offset;
        Size = size;
    }

    public TransferBuffer(byte[] buffer, int offset, int size, bool assign)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        Buffer = assign ? buffer : [.. buffer];
        Offset = offset;
        Size = size;
    }

    public TransferBuffer(TransferBuffer rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);
        Buffer = [.. rhs.Buffer];
        Offset = rhs.Offset;
        Size = rhs.Size;
    }

    public Span<byte> AsSpan() => Buffer.AsSpan(Offset, Size);
    public ReadOnlySpan<byte> AsReadOnlySpan() => Buffer.AsSpan(Offset, Size);
}