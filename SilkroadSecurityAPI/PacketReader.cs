using System.Buffers.Binary;

namespace SilkroadSecurityAPI;

/// <summary>
/// Zero-copy, little-endian reader over a byte buffer.
/// <para>
/// Stored as a mutable field on <see cref="Packet"/>, so it cannot be a
/// <c>ref struct</c>. Instead it holds a <see cref="ReadOnlyMemory{T}"/> and
/// exposes spans on demand.
/// </para>
/// </summary>
internal struct PacketReader(ReadOnlyMemory<byte> data)
{
    private readonly ReadOnlyMemory<byte> _data = data;
    private int _pos = 0;

    public PacketReader(byte[] data) : this(data.AsMemory()) { }

    public PacketReader(byte[] data, int offset, int count)
        : this(data.AsMemory(offset, count)) { }

    public readonly int Position => _pos;
    public readonly int Remaining => _data.Length - _pos;
    public readonly int Length => _data.Length;

    private ReadOnlySpan<byte> Take(int n)
    {
        if ((uint)_pos + (uint)n > (uint)_data.Length)
            throw new SecurityException(
                $"[PacketReader] Need {n} byte(s), only {Remaining} available.");
        var s = _data.Span.Slice(_pos, n);
        _pos += n;
        return s;
    }

    public byte ReadByte() => Take(1)[0];
    public sbyte ReadSByte() => unchecked((sbyte)ReadByte());
    public ushort ReadUInt16() => BinaryPrimitives.ReadUInt16LittleEndian(Take(2));
    public short ReadInt16() => BinaryPrimitives.ReadInt16LittleEndian(Take(2));
    public uint ReadUInt32() => BinaryPrimitives.ReadUInt32LittleEndian(Take(4));
    public int ReadInt32() => BinaryPrimitives.ReadInt32LittleEndian(Take(4));
    public ulong ReadUInt64() => BinaryPrimitives.ReadUInt64LittleEndian(Take(8));
    public long ReadInt64() => BinaryPrimitives.ReadInt64LittleEndian(Take(8));
    public float ReadSingle() => BitConverter.UInt32BitsToSingle(ReadUInt32());
    public double ReadDouble() => BitConverter.UInt64BitsToDouble(ReadUInt64());

    public bool TryReadUInt16(out ushort value)
    {
        if (Remaining < 2) { value = 0; return false; }
        value = BinaryPrimitives.ReadUInt16LittleEndian(_data.Span.Slice(_pos, 2));
        _pos += 2;
        return true;
    }

    public ReadOnlySpan<byte> ReadBytes(int count) => Take(count);
}