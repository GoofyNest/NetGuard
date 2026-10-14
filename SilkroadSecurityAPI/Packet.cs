using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Text;

namespace SilkroadSecurityAPI;

/// <summary>
/// A Silkroad protocol packet. Packets start as writeable; call
/// <see cref="Lock"/> to freeze the byte buffer and enable reading.
/// </summary>
public sealed class Packet
{
    private static readonly ConcurrentDictionary<int, Encoding> EncodingCache = new();

    private static Encoding GetEncoding(int codepage) =>
        EncodingCache.GetOrAdd(codepage, static cp =>
            CodePagesEncodingProvider.Instance.GetEncoding(cp)
            ?? throw new SecurityException($"Unknown codepage {cp}."));

    private bool _isReadOnly;
    private PacketWriter? _writer;
    private PacketReader _reader;
    private byte[]? _readerBytes;

    public ushort Opcode { get; }
    public bool Encrypted { get; }
    public bool Massive { get; }

    // -- Constructors -------------------------------------------------------

    public Packet(ushort opcode)
        : this(opcode, encrypted: false, massive: false) { }

    public Packet(ushort opcode, bool encrypted)
        : this(opcode, encrypted, massive: false) { }

    public Packet(ushort opcode, bool encrypted, bool massive)
    {
        if (encrypted && massive)
            throw new SecurityException(
                "[Packet] Packets cannot both be massive and encrypted.");

        Opcode = opcode;
        Encrypted = encrypted;
        Massive = massive;
        _writer = new PacketWriter();
    }

    public Packet(ushort opcode, bool encrypted, bool massive, ReadOnlySpan<byte> bytes)
    {
        if (encrypted && massive)
            throw new SecurityException(
                "[Packet] Packets cannot both be massive and encrypted.");

        Opcode = opcode;
        Encrypted = encrypted;
        Massive = massive;
        _writer = new PacketWriter(bytes.Length < 1 ? 1 : bytes.Length);
        _writer.Write(bytes);
    }

    public Packet(ushort opcode, bool encrypted, bool massive,
                  byte[] bytes, int offset, int length)
        : this(opcode, encrypted, massive,
               bytes.AsSpan(offset, length))
    { }

    public Packet(Packet packet)
    {
        ArgumentNullException.ThrowIfNull(packet);

        Opcode = packet.Opcode;
        Encrypted = packet.Encrypted;
        Massive = packet.Massive;

        _isReadOnly = packet._isReadOnly;
        if (_isReadOnly)
        {
            _readerBytes = [.. packet._readerBytes!];
            _reader = new PacketReader(_readerBytes);
        }
        else
        {
            var src = packet._writer!;
            _writer = new PacketWriter(src.Length < 1 ? 1 : src.Length);
            _writer.Write(src.Span);
        }
    }

    // -- Lifecycle ----------------------------------------------------------

    public byte[] GetBytes()
    {
        if (_isReadOnly) return _readerBytes!;
        return _writer!.ToArray();
    }

    /// <summary>Freezes the packet and enables reading. Idempotent.</summary>
    public void Lock()
    {
        if (_isReadOnly) return;

        _readerBytes = _writer!.ToArray();
        _writer.Dispose();
        _writer = null;
        _reader = new PacketReader(_readerBytes);
        _isReadOnly = true;
    }

    private void EnsureReadable()
    {
        if (!_isReadOnly)
            throw new InvalidOperationException("Cannot read from an unlocked Packet.");
    }

    private void EnsureWritable()
    {
        if (_isReadOnly)
            throw new InvalidOperationException("Cannot write to a locked Packet.");
    }

    public int RemainingRead()
    {
        EnsureReadable();
        return _reader.Remaining;
    }

    // -- Reads --------------------------------------------------------------

    public byte ReadUInt8() { EnsureReadable(); return _reader.ReadByte(); }
    public sbyte ReadInt8() { EnsureReadable(); return _reader.ReadSByte(); }
    public ushort ReadUInt16() { EnsureReadable(); return _reader.ReadUInt16(); }
    public short ReadInt16() { EnsureReadable(); return _reader.ReadInt16(); }
    public uint ReadUInt32() { EnsureReadable(); return _reader.ReadUInt32(); }
    public int ReadInt32() { EnsureReadable(); return _reader.ReadInt32(); }
    public ulong ReadUInt64() { EnsureReadable(); return _reader.ReadUInt64(); }
    public long ReadInt64() { EnsureReadable(); return _reader.ReadInt64(); }
    public float ReadSingle() { EnsureReadable(); return _reader.ReadSingle(); }
    public double ReadDouble() { EnsureReadable(); return _reader.ReadDouble(); }

    public byte[] ReadUInt8Array(int count)
    {
        EnsureReadable();
        return _reader.ReadBytes(count).ToArray();
    }

    public string ReadAscii(int codepage = 1252)
    {
        EnsureReadable();
        ushort length = _reader.ReadUInt16();
        var bytes = _reader.ReadBytes(length);
        return GetEncoding(codepage).GetString(bytes);
    }

    public string ReadUnicode()
    {
        EnsureReadable();
        ushort length = _reader.ReadUInt16();
        var bytes = _reader.ReadBytes(length * 2);
        return Encoding.Unicode.GetString(bytes);
    }

    public string[] ReadAsciiArray(int count, int codepage = 1252)
    {
        EnsureReadable();
        var values = new string[count];
        for (int i = 0; i < count; i++)
            values[i] = ReadAscii(codepage);
        return values;
    }

    public string[] ReadUnicodeArray(int count)
    {
        EnsureReadable();
        var values = new string[count];
        for (int i = 0; i < count; i++)
            values[i] = ReadUnicode();
        return values;
    }

    public float ReadFloat() => ReadSingle();

    public sbyte[] ReadInt8Array(int count)
    {
        EnsureReadable();
        var v = new sbyte[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadSByte();
        return v;
    }

    public ushort[] ReadUInt16Array(int count)
    {
        EnsureReadable();
        var v = new ushort[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadUInt16();
        return v;
    }

    public short[] ReadInt16Array(int count)
    {
        EnsureReadable();
        var v = new short[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadInt16();
        return v;
    }

    public uint[] ReadUInt32Array(int count)
    {
        EnsureReadable();
        var v = new uint[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadUInt32();
        return v;
    }

    public int[] ReadInt32Array(int count)
    {
        EnsureReadable();
        var v = new int[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadInt32();
        return v;
    }

    public ulong[] ReadUInt64Array(int count)
    {
        EnsureReadable();
        var v = new ulong[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadUInt64();
        return v;
    }

    public long[] ReadInt64Array(int count)
    {
        EnsureReadable();
        var v = new long[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadInt64();
        return v;
    }

    public float[] ReadSingleArray(int count)
    {
        EnsureReadable();
        var v = new float[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadSingle();
        return v;
    }

    public double[] ReadDoubleArray(int count)
    {
        EnsureReadable();
        var v = new double[count];
        for (int i = 0; i < count; i++) v[i] = _reader.ReadDouble();
        return v;
    }

    // -- Writes -------------------------------------------------------------

    public void WriteUInt8(byte v) { EnsureWritable(); _writer!.WriteByte(v); }
    public void WriteInt8(sbyte v) { EnsureWritable(); _writer!.WriteByte(unchecked((byte)v)); }
    public void WriteUInt16(ushort v) { EnsureWritable(); _writer!.WriteUInt16(v); }
    public void WriteInt16(short v) { EnsureWritable(); _writer!.WriteInt16(v); }
    public void WriteUInt32(uint v) { EnsureWritable(); _writer!.WriteUInt32(v); }
    public void WriteInt32(int v) { EnsureWritable(); _writer!.WriteInt32(v); }
    public void WriteUInt64(ulong v) { EnsureWritable(); _writer!.WriteUInt64(v); }
    public void WriteInt64(long v) { EnsureWritable(); _writer!.WriteInt64(v); }
    public void WriteSingle(float v) { EnsureWritable(); _writer!.WriteSingle(v); }
    public void WriteDouble(double v) { EnsureWritable(); _writer!.WriteDouble(v); }
    public void WriteFloat(float v) => WriteSingle(v);

    public void WriteUInt8Array(ReadOnlySpan<byte> values)
    {
        EnsureWritable();
        _writer!.Write(values);
    }

    public void WriteAscii(string value, int codepage = 1252)
    {
        ArgumentNullException.ThrowIfNull(value);
        EnsureWritable();

        var encoding = GetEncoding(codepage);
        int max = encoding.GetMaxByteCount(value.Length);
        byte[] rented = System.Buffers.ArrayPool<byte>.Shared.Rent(Math.Max(max, 1));
        try
        {
            int n = encoding.GetBytes(value.AsSpan(), rented);
            _writer!.WriteUInt16((ushort)n);
            _writer.Write(rented.AsSpan(0, n));
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public void WriteUnicode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        EnsureWritable();

        int max = Encoding.Unicode.GetMaxByteCount(value.Length);
        byte[] rented = System.Buffers.ArrayPool<byte>.Shared.Rent(Math.Max(max, 1));
        try
        {
            int n = Encoding.Unicode.GetBytes(value.AsSpan(), rented);
            _writer!.WriteUInt16((ushort)value.Length);
            _writer.Write(rented.AsSpan(0, n));
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public void WriteUInt16Array(ReadOnlySpan<ushort> values)
    {
        EnsureWritable();
        foreach (var v in values) _writer!.WriteUInt16(v);
    }

    public void WriteUInt32Array(ReadOnlySpan<uint> values)
    {
        EnsureWritable();
        foreach (var v in values) _writer!.WriteUInt32(v);
    }

    public void WriteAsciiArray(ReadOnlySpan<string> values, int codepage = 1252)
    {
        EnsureWritable();
        foreach (var v in values) WriteAscii(v, codepage);
    }

    public void WriteUnicodeArray(ReadOnlySpan<string> values)
    {
        EnsureWritable();
        foreach (var v in values) WriteUnicode(v);
    }

    // -- Internal bridge for Security.cs ------------------------------------

    /// <summary>
    /// View of the packet's current payload.
    /// Do not retain past <see cref="Lock"/> — the underlying buffer may be
    /// returned to the pool, making the span invalid.
    /// </summary>
    internal ReadOnlySpan<byte> PayloadSpan
    {
        get
        {
            if (_isReadOnly) return _readerBytes.AsSpan();
            return _writer!.Span;
        }
    }
}