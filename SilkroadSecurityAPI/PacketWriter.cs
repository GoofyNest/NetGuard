using System.Buffers;
using System.Buffers.Binary;

namespace SilkroadSecurityAPI;

/// <summary>
/// Growable, pooled little-endian binary writer.
/// Replaces the old <c>BinaryWriter</c> + <c>MemoryStream</c> combo.
/// Not thread-safe.
/// </summary>
internal sealed class PacketWriter : IDisposable
{
    private byte[]? _buffer;
    private int _length;

    public PacketWriter(int initialCapacity = 256)
    {
        if (initialCapacity < 1) initialCapacity = 1;
        _buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        _length = 0;
    }

    public int Length => _length;
    public ReadOnlySpan<byte> Span => _buffer.AsSpan(0, _length);

    private void EnsureCapacity(int additional)
    {
        int required = _length + additional;
        if (required <= _buffer!.Length) return;

        int newSize = (int)System.Numerics.BitOperations.RoundUpToPowerOf2((uint)Math.Max(required, 16));
        byte[] newBuf = ArrayPool<byte>.Shared.Rent(newSize);
        _buffer.AsSpan(0, _length).CopyTo(newBuf);
        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = newBuf;
    }

    public void WriteByte(byte value)
    {
        EnsureCapacity(1);
        _buffer![_length++] = value;
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty) return;
        EnsureCapacity(data.Length);
        data.CopyTo(_buffer.AsSpan(_length));
        _length += data.Length;
    }

    public void WriteUInt16(ushort value)
    {
        EnsureCapacity(2);
        BinaryPrimitives.WriteUInt16LittleEndian(_buffer.AsSpan(_length), value);
        _length += 2;
    }

    public void WriteUInt32(uint value)
    {
        EnsureCapacity(4);
        BinaryPrimitives.WriteUInt32LittleEndian(_buffer.AsSpan(_length), value);
        _length += 4;
    }

    public void WriteUInt64(ulong value)
    {
        EnsureCapacity(8);
        BinaryPrimitives.WriteUInt64LittleEndian(_buffer.AsSpan(_length), value);
        _length += 8;
    }

    public void WriteInt16(short v) => WriteUInt16(unchecked((ushort)v));
    public void WriteInt32(int v) => WriteUInt32(unchecked((uint)v));
    public void WriteInt64(long v) => WriteUInt64(unchecked((ulong)v));
    public void WriteSingle(float v) => WriteUInt32(BitConverter.SingleToUInt32Bits(v));
    public void WriteDouble(double v) => WriteUInt64(BitConverter.DoubleToUInt64Bits(v));

    /// <summary>Overwrite 2 bytes at <paramref name="offset"/>; the region must already exist.</summary>
    public void PatchUInt16(int offset, ushort value)
    {
        if ((uint)offset + 2u > (uint)_length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        BinaryPrimitives.WriteUInt16LittleEndian(_buffer.AsSpan(offset), value);
    }

    /// <summary>Overwrite 1 byte at <paramref name="offset"/>; the region must already exist.</summary>
    public void PatchByte(int offset, byte value)
    {
        if ((uint)offset >= (uint)_length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        _buffer![offset] = value;
    }

    public Span<byte> WritableSpan => _buffer.AsSpan(0, _length);

    /// <summary>
    /// Extends the writer with zero bytes so that the region
    /// <c>[sliceStart, Length)</c> has a length that is a multiple of <paramref name="alignment"/>.
    /// </summary>
    public void PadSoSliceIsAligned(int sliceStart, int alignment)
    {
        if (alignment <= 0 || (alignment & (alignment - 1)) != 0)
            throw new ArgumentOutOfRangeException(nameof(alignment), "Alignment must be a positive power of two.");
        if ((uint)sliceStart > (uint)_length)
            throw new ArgumentOutOfRangeException(nameof(sliceStart));

        int bodyLen = _length - sliceStart;
        int pad = ((bodyLen + alignment - 1) & ~(alignment - 1)) - bodyLen;
        if (pad <= 0) return;

        EnsureCapacity(pad);
        _buffer.AsSpan(_length, pad).Clear();
        _length += pad;
    }

    public byte[] ToArray() => _buffer.AsSpan(0, _length).ToArray();

    public void Dispose()
    {
        if (_buffer is not null)
        {
            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = null;
            _length = 0;
        }
    }
}