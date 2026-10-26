using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace SilkroadSecurityAPI;

public sealed class Security
{
    // ---------------------------------------------------------------------
    // Security table (CRC-32 lookup used by the Silkroad handshake)
    // ---------------------------------------------------------------------

    private static readonly uint[] GlobalSecurityTable = GenerateSecurityTable();

    private static uint[] GenerateSecurityTable()
    {
        ReadOnlySpan<byte> baseTable =
        [
            0xB1,0xD6,0x8B,0x96,0x96,0x30,0x07,0x77,0x2C,0x61,0x0E,0xEE,0xBA,0x51,0x09,0x99,
            0x19,0xC4,0x6D,0x07,0x8F,0xF4,0x6A,0x70,0x35,0xA5,0x63,0xE9,0xA3,0x95,0x64,0x9E,
            0x32,0x88,0xDB,0x0E,0xA4,0xB8,0xDC,0x79,0x1E,0xE9,0xD5,0xE0,0x88,0xD9,0xD2,0x97,
            0x2B,0x4C,0xB6,0x09,0xBD,0x7C,0xB1,0x7E,0x07,0x2D,0xB8,0xE7,0x91,0x1D,0xBF,0x90,
            0x64,0x10,0xB7,0x1D,0xF2,0x20,0xB0,0x6A,0x48,0x71,0xB1,0xF3,0xDE,0x41,0xBE,0x8C,
            0x7D,0xD4,0xDA,0x1A,0xEB,0xE4,0xDD,0x6D,0x51,0xB5,0xD4,0xF4,0xC7,0x85,0xD3,0x83,
            0x56,0x98,0x6C,0x13,0xC0,0xA8,0x6B,0x64,0x7A,0xF9,0x62,0xFD,0xEC,0xC9,0x65,0x8A,
            0x4F,0x5C,0x01,0x14,0xD9,0x6C,0x06,0x63,0x63,0x3D,0x0F,0xFA,0xF5,0x0D,0x08,0x8D,
            0xC8,0x20,0x6E,0x3B,0x5E,0x10,0x69,0x4C,0xE4,0x41,0x60,0xD5,0x72,0x71,0x67,0xA2,
            0xD1,0xE4,0x03,0x3C,0x47,0xD4,0x04,0x4B,0xFD,0x85,0x0D,0xD2,0x6B,0xB5,0x0A,0xA5,
            0xFA,0xA8,0xB5,0x35,0x6C,0x98,0xB2,0x42,0xD6,0xC9,0xBB,0xDB,0x40,0xF9,0xBC,0xAC,
            0xE3,0x6C,0xD8,0x32,0x75,0x5C,0xDF,0x45,0xCF,0x0D,0xD6,0xDC,0x59,0x3D,0xD1,0xAB,
            0xAC,0x30,0xD9,0x26,0x3A,0x00,0xDE,0x51,0x80,0x51,0xD7,0xC8,0x16,0x61,0xD0,0xBF,
            0xB5,0xF4,0xB4,0x21,0x23,0xC4,0xB3,0x56,0x99,0x95,0xBA,0xCF,0x0F,0xA5,0xB7,0xB8,
            0x9E,0xB8,0x02,0x28,0x08,0x88,0x05,0x5F,0xB2,0xD9,0xEC,0xC6,0x24,0xE9,0x0B,0xB1,
            0x87,0x7C,0x6F,0x2F,0x11,0x4C,0x68,0x58,0xAB,0x1D,0x61,0xC1,0x3D,0x2D,0x66,0xB6,
            0x90,0x41,0xDC,0x76,0x06,0x71,0xDB,0x01,0xBC,0x20,0xD2,0x98,0x2A,0x10,0xD5,0xEF,
            0x89,0x85,0xB1,0x71,0x1F,0xB5,0xB6,0x06,0xA5,0xE4,0xBF,0x9F,0x33,0xD4,0xB8,0xE8,
            0xA2,0xC9,0x07,0x78,0x34,0xF9,0xA0,0x0F,0x8E,0xA8,0x09,0x96,0x18,0x98,0x0E,0xE1,
            0xBB,0x0D,0x6A,0x7F,0x2D,0x3D,0x6D,0x08,0x97,0x6C,0x64,0x91,0x01,0x5C,0x63,0xE6,
            0xF4,0x51,0x6B,0x6B,0x62,0x61,0x6C,0x1C,0xD8,0x30,0x65,0x85,0x4E,0x00,0x62,0xF2,
            0xED,0x95,0x06,0x6C,0x7B,0xA5,0x01,0x1B,0xC1,0xF4,0x08,0x82,0x57,0xC4,0x0F,0xF5,
            0xC6,0xD9,0xB0,0x63,0x50,0xE9,0xB7,0x12,0xEA,0xB8,0xBE,0x8B,0x7C,0x88,0xB9,0xFC,
            0xDF,0x1D,0xDD,0x62,0x49,0x2D,0xDA,0x15,0xF3,0x7C,0xD3,0x8C,0x65,0x4C,0xD4,0xFB,
            0x58,0x61,0xB2,0x4D,0xCE,0x51,0xB5,0x3A,0x74,0x00,0xBC,0xA3,0xE2,0x30,0xBB,0xD4,
            0x41,0xA5,0xDF,0x4A,0xD7,0x95,0xD8,0x3D,0x6D,0xC4,0xD1,0xA4,0xFB,0xF4,0xD6,0xD3,
            0x6A,0xE9,0x69,0x43,0xFC,0xD9,0x6E,0x34,0x46,0x88,0x67,0xAD,0xD0,0xB8,0x60,0xDA,
            0x73,0x2D,0x04,0x44,0xE5,0x1D,0x03,0x33,0x5F,0x4C,0x0A,0xAA,0xC9,0x7C,0x0D,0xDD,
            0x3C,0x71,0x05,0x50,0xAA,0x41,0x02,0x27,0x10,0x10,0x0B,0xBE,0x86,0x20,0x0C,0xC9,
            0x25,0xB5,0x68,0x57,0xB3,0x85,0x6F,0x20,0x09,0xD4,0x66,0xB9,0x9F,0xE4,0x61,0xCE,
            0x0E,0xF9,0xDE,0x5E,0x08,0xC9,0xD9,0x29,0x22,0x98,0xD0,0xB0,0xB4,0xA8,0x57,0xC7,
            0x17,0x3D,0xB3,0x59,0x81,0x0D,0xB4,0x3E,0x3B,0x5C,0xBD,0xB7,0xAD,0x6C,0xBA,0xC0,
            0x20,0x83,0xB8,0xED,0xB6,0xB3,0xBF,0x9A,0x0C,0xE2,0xB6,0x03,0x9A,0xD2,0xB1,0x74,
            0x39,0x47,0xD5,0xEA,0xAF,0x77,0xD2,0x9D,0x15,0x26,0xDB,0x04,0x83,0x16,0xDC,0x73,
            0x12,0x0B,0x63,0xE3,0x84,0x3B,0x64,0x94,0x3E,0x6A,0x6D,0x0D,0xA8,0x5A,0x6A,0x7A,
            0x0B,0xCF,0x0E,0xE4,0x9D,0xFF,0x09,0x93,0x27,0xAE,0x00,0x0A,0xB1,0x9E,0x07,0x7D,
            0x44,0x93,0x0F,0xF0,0xD2,0xA2,0x08,0x87,0x68,0xF2,0x01,0x1E,0xFE,0xC2,0x06,0x69,
            0x5D,0x57,0x62,0xF7,0xCB,0x67,0x65,0x80,0x71,0x36,0x6C,0x19,0xE7,0x06,0x6B,0x6E,
            0x76,0x1B,0xD4,0xFE,0xE0,0x2B,0xD3,0x89,0x5A,0x7A,0xDA,0x10,0xCC,0x4A,0xDD,0x67,
            0x6F,0xDF,0xB9,0xF9,0xF9,0xEF,0xBE,0x8E,0x43,0xBE,0xB7,0x17,0xD5,0x8E,0xB0,0x60,
            0xE8,0xA3,0xD6,0xD6,0x7E,0x93,0xD1,0xA1,0xC4,0xC2,0xD8,0x38,0x52,0xF2,0xDF,0x4F,
            0xF1,0x67,0xBB,0xD1,0x67,0x57,0xBC,0xA6,0xDD,0x06,0xB5,0x3F,0x4B,0x36,0xB2,0x48,
            0xDA,0x2B,0x0D,0xD8,0x4C,0x1B,0x0A,0xAF,0xF6,0x4A,0x03,0x36,0x60,0x7A,0x04,0x41,
            0xC3,0xEF,0x60,0xDF,0x55,0xDF,0x67,0xA8,0xEF,0x8E,0x6E,0x31,0x79,0x0E,0x69,0x46,
            0x8C,0xB3,0x51,0xCB,0x1A,0x83,0x63,0xBC,0xA0,0xD2,0x6F,0x25,0x36,0xE2,0x68,0x52,
            0x95,0x77,0x0C,0xCC,0x03,0x47,0x0B,0xBB,0xB9,0x14,0x02,0x22,0x2F,0x26,0x05,0x55,
            0xBE,0x3B,0xB6,0xC5,0x28,0x0B,0xBD,0xB2,0x92,0x5A,0xB4,0x2B,0x04,0x6A,0xB3,0x5C,
            0xA7,0xFF,0xD7,0xC2,0x31,0xCF,0xD0,0xB5,0x8B,0x9E,0xD9,0x2C,0x1D,0xAE,0xDE,0x5B,
            0xB0,0x72,0x64,0x9B,0x26,0xF2,0xE3,0xEC,0x9C,0xA3,0x6A,0x75,0x0A,0x93,0x6D,0x02,
            0xA9,0x06,0x09,0x9C,0x3F,0x36,0x0E,0xEB,0x85,0x68,0x07,0x72,0x13,0x07,0x00,0x05,
            0x82,0x48,0xBF,0x95,0x14,0x7A,0xB8,0xE2,0xAE,0x2B,0xB1,0x7B,0x38,0x1B,0xB6,0x0C,
            0x9B,0x8E,0xD2,0x92,0x0D,0xBE,0xD5,0xE5,0xB7,0xEF,0xDC,0x7C,0x21,0xDF,0xDB,0x0B,
            0x94,0xD2,0xD3,0x86,0x42,0xE2,0xD4,0xF1,0xF8,0xB3,0xDD,0x68,0x6E,0x83,0xDA,0x1F,
            0xCD,0x16,0xBE,0x81,0x5B,0x26,0xB9,0xF6,0xE1,0x77,0xB0,0x6F,0x77,0x47,0xB7,0x18,
            0xE0,0x5A,0x08,0x88,0x70,0x6A,0x0F,0xF1,0xCA,0x3B,0x06,0x66,0x5C,0x0B,0x01,0x11,
            0xFF,0x9E,0x65,0x8F,0x69,0xAE,0x62,0xF8,0xD3,0xFF,0x6B,0x61,0x45,0xCF,0x6C,0x16,
            0x78,0xE2,0x0A,0xA0,0xEE,0xD2,0x0D,0xD7,0x54,0x83,0x04,0x4E,0xC2,0xB3,0x03,0x39,
            0x61,0x26,0x67,0xA7,0xF7,0x16,0x60,0xD0,0x4D,0x47,0x69,0x49,0xDB,0x77,0x6E,0x3E,
            0x4A,0x6A,0xD1,0xAE,0xDC,0x5A,0xD6,0xD9,0x66,0x0B,0xDF,0x40,0xF0,0x3B,0xD8,0x37,
            0x53,0xAE,0xBC,0xA9,0xC5,0x9E,0xBB,0xDE,0x7F,0xCF,0xB2,0x47,0xE9,0xFF,0xB5,0x30,
            0x1C,0xF9,0xBD,0xBD,0x8A,0xCD,0xBA,0xCA,0x30,0x9E,0xB3,0x53,0xA6,0xA3,0xBC,0x24,
            0x05,0x3B,0xD0,0xBA,0xA3,0x06,0xD7,0xCD,0xE9,0x57,0xDE,0x54,0xBF,0x67,0xD9,0x23,
            0x2E,0x72,0x66,0xB3,0xB8,0x4A,0x61,0xC4,0x02,0x1B,0x38,0x5D,0x94,0x2B,0x6F,0x2B,
            0x37,0xBE,0xCB,0xB4,0xA1,0x8E,0xCC,0xC3,0x1B,0xDF,0x0D,0x5A,0x8D,0xED,0x02,0x2D,
        ];

        var table = new uint[0x10000];
        int index = 0;
        for (int edi = 0; edi < 1024; edi += 4)
        {
            uint edx = BinaryPrimitives.ReadUInt32LittleEndian(baseTable[edi..]);
            for (uint ecx = 0; ecx < 256; ecx++)
            {
                uint eax = ecx >> 1;
                if ((ecx & 1) != 0) eax ^= edx;
                for (int bit = 0; bit < 7; bit++)
                    eax = (eax & 1) != 0 ? (eax >> 1) ^ edx : eax >> 1;
                table[index++] = eax;
            }
        }
        return table;
    }

    // ---------------------------------------------------------------------
    // State
    // ---------------------------------------------------------------------

    private uint _valueX, _valueG, _valueP, _valueA, _valueB, _valueK;
    private uint _seedCount, _crcSeed;
    private ulong _initialBlowfishKey, _handshakeBlowfishKey;
    private readonly byte[] _countByteSeeds = new byte[3];
    private ulong _clientKey, _challengeKey;

    private bool _clientSecurity;
    private byte _securityFlagByte;
    private SecurityFlag _securityFlags;
    private bool _acceptedHandshake;
    private bool _startedHandshake;
    private byte _identityFlag;
    private string _identityName;

    private List<Packet> _incomingPackets = [];
    private readonly List<Packet> _outgoingPackets = [];

    private readonly List<ushort> _encryptedOpcodes;
    private readonly Blowfish _blowfish = new();

    private readonly TransferBuffer _recvBuffer;
    private TransferBuffer? _currentBuffer;

    private ushort _massiveCount;
    private Packet? _massivePacket;

    // ---------------------------------------------------------------------
    // Construction
    // ---------------------------------------------------------------------

    public Security()
    {
        _countByteSeeds[0] = 0;
        _countByteSeeds[1] = 0;
        _countByteSeeds[2] = 0;
        _identityName = "SR_Client";

        _encryptedOpcodes =
        [
            0x2001,
            0x6100,
            0x6101,
            0x6102,
            0x6103,
            0x6107,
        ];

        _recvBuffer = new TransferBuffer(8192);
    }

    public void ChangeIdentity(string name, byte flag)
    {
        ArgumentNullException.ThrowIfNull(name);
        _identityName = name;
        _identityFlag = flag;
    }

    public void AddEncryptedOpcode(ushort opcode)
    {
        if (!_encryptedOpcodes.Contains(opcode))
            _encryptedOpcodes.Add(opcode);
    }

    // ---------------------------------------------------------------------
    // Crypto primitives (must match the wire protocol exactly)
    // ---------------------------------------------------------------------

    private static uint GenerateValue(ref uint val)
    {
        for (int i = 0; i < 32; i++)
        {
            val = (((((((((((val >> 2) ^ val) >> 2) ^ val) >> 1) ^ val) >> 1) ^ val) >> 1) ^ val) & 1)
                | ((((val & 1) << 31) | (val >> 1)) & 0xFFFFFFFE);
        }
        return val;
    }

    private void SetupCountByte(uint seed)
    {
        if (seed == 0) seed = 0x9ABFB3B6;
        uint mut = seed;
        uint mut1 = GenerateValue(ref mut);
        uint mut2 = GenerateValue(ref mut);
        uint mut3 = GenerateValue(ref mut);
        GenerateValue(ref mut);

        byte b1 = (byte)((mut & 0xFF) ^ (mut3 & 0xFF));
        byte b2 = (byte)((mut1 & 0xFF) ^ (mut2 & 0xFF));
        if (b1 == 0) b1 = 1;
        if (b2 == 0) b2 = 1;

        _countByteSeeds[0] = (byte)(b1 ^ b2);
        _countByteSeeds[1] = b2;
        _countByteSeeds[2] = b1;
    }

    private static uint GPowXModP(uint p, uint x, uint g)
    {
        long result = 1;
        long mult = g;
        if (x == 0) return 1;
        while (x != 0)
        {
            if ((x & 1) > 0) result = (mult * result) % p;
            x >>= 1;
            mult = (mult * mult) % p;
        }
        return (uint)result;
    }

    private static void KeyTransformValue(ref ulong val, uint key, byte keyByte)
    {
        Span<byte> stream = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(stream, val);

        stream[0] ^= (byte)(stream[0] + (byte)(key & 0xFF) + keyByte);
        stream[1] ^= (byte)(stream[1] + (byte)((key >> 8) & 0xFF) + keyByte);
        stream[2] ^= (byte)(stream[2] + (byte)((key >> 16) & 0xFF) + keyByte);
        stream[3] ^= (byte)(stream[3] + (byte)((key >> 24) & 0xFF) + keyByte);
        stream[4] ^= (byte)(stream[4] + (byte)(key & 0xFF) + keyByte);
        stream[5] ^= (byte)(stream[5] + (byte)((key >> 8) & 0xFF) + keyByte);
        stream[6] ^= (byte)(stream[6] + (byte)((key >> 16) & 0xFF) + keyByte);
        stream[7] ^= (byte)(stream[7] + (byte)((key >> 24) & 0xFF) + keyByte);

        val = BinaryPrimitives.ReadUInt64LittleEndian(stream);
    }

    private byte GenerateCountByte(bool update)
    {
        byte result = (byte)(_countByteSeeds[2] * (~_countByteSeeds[0] + _countByteSeeds[1]));
        result = (byte)(result ^ (result >> 4));
        if (update) _countByteSeeds[0] = result;
        return result;
    }

    private byte GenerateCheckByte(ReadOnlySpan<byte> stream)
    {
        uint checksum = 0xFFFFFFFF;
        uint moddedSeed = _crcSeed << 8;
        for (int x = 0; x < stream.Length; x++)
        {
            checksum = (checksum >> 8) ^ GlobalSecurityTable[moddedSeed + ((stream[x] ^ checksum) & 0xFF)];
        }
        return (byte)(((checksum >> 24) & 0xFF) + ((checksum >> 8) & 0xFF)
                    + ((checksum >> 16) & 0xFF) + (checksum & 0xFF));
    }

    // Crypto-grade RNG — replaces the old System.Random.
    private static ulong NextUInt64()
    {
        Span<byte> b = stackalloc byte[8];
        RandomNumberGenerator.Fill(b);
        return BinaryPrimitives.ReadUInt64LittleEndian(b);
    }

    private static uint NextUInt32() => (uint)(NextUInt64() & 0xFFFFFFFF);

    private static byte NextUInt8() => (byte)(NextUInt64() & 0xFF);

    // ---------------------------------------------------------------------
    // Security negotiation
    // ---------------------------------------------------------------------

    public void GenerateSecurity(bool blowfish, bool securityBytes, bool handshake)
    {
        SecurityFlag flags = SecurityFlag.None;
        if (blowfish) flags |= SecurityFlag.Blowfish;
        if (securityBytes) flags |= SecurityFlag.SecurityBytes;
        if (handshake) flags |= SecurityFlag.Handshake;
        if (!blowfish && !securityBytes && !handshake) flags = SecurityFlag.None;
        GenerateSecurity(flags);
    }

    private void GenerateSecurity(SecurityFlag flags)
    {
        _securityFlagByte = (byte)flags;
        _securityFlags = flags;
        _clientSecurity = true;

        using var response = new PacketWriter();
        response.WriteByte(_securityFlagByte);

        if (flags.HasFlag(SecurityFlag.Blowfish))
        {
            _initialBlowfishKey = NextUInt64();
            Span<byte> keyBytes = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(keyBytes, _initialBlowfishKey);
            _blowfish.Initialize(keyBytes);
            response.WriteUInt64(_initialBlowfishKey);
        }
        if (flags.HasFlag(SecurityFlag.SecurityBytes))
        {
            _seedCount = NextUInt8();
            SetupCountByte(_seedCount);
            _crcSeed = NextUInt8();
            response.WriteUInt32(_seedCount);
            response.WriteUInt32(_crcSeed);
        }
        if (flags.HasFlag(SecurityFlag.Handshake))
        {
            _handshakeBlowfishKey = NextUInt64();
            _valueX = NextUInt32() & 0x7FFFFFFF;
            _valueG = NextUInt32() & 0x7FFFFFFF;
            _valueP = NextUInt32() & 0x7FFFFFFF;
            _valueA = GPowXModP(_valueP, _valueX, _valueG);

            response.WriteUInt64(_handshakeBlowfishKey);
            response.WriteUInt32(_valueG);
            response.WriteUInt32(_valueP);
            response.WriteUInt32(_valueA);
        }

        _outgoingPackets.Add(new Packet(0x5000, false, false, response.Span));
    }

    private void Handshake(ushort opcode, ref PacketReader reader, bool encrypted)
    {
        if (encrypted)
            throw new SecurityException("[Security::Handshake] Received an encrypted handshake packet.");

        if (_clientSecurity)
            ClientHandshake(opcode, ref reader);
        else
            ServerHandshake(opcode, ref reader);
    }

    private void ClientHandshake(ushort opcode, ref PacketReader reader)
    {
        if (!_securityFlags.HasFlag(SecurityFlag.Handshake))
        {
            if (opcode == 0x9000)
            {
                if (_acceptedHandshake)
                    throw new SecurityException("[Security::Handshake] Duplicate 0x9000.");
                _acceptedHandshake = true;
                return;
            }
            if (opcode == 0x5000)
                throw new SecurityException("[Security::Handshake] Unexpected 0x5000 (no handshake).");
            throw new SecurityException("[Security::Handshake] Unexpected handshake opcode.");
        }

        if (opcode == 0x9000)
        {
            if (!_startedHandshake)
                throw new SecurityException("[Security::Handshake] Out-of-order 0x9000.");
            if (_acceptedHandshake)
                throw new SecurityException("[Security::Handshake] Duplicate 0x9000.");
            _acceptedHandshake = true;
            return;
        }

        if (opcode != 0x5000)
            throw new SecurityException("[Security::Handshake] Unexpected handshake opcode.");

        if (_startedHandshake)
            throw new SecurityException("[Security::Handshake] Duplicate 0x5000.");

        _startedHandshake = true;

        _valueB = reader.ReadUInt32();
        _clientKey = reader.ReadUInt64();

        _valueK = GPowXModP(_valueP, _valueX, _valueB);

        // Verify client signature
        ulong keyArray = MakeLongLong(_valueA, _valueB);
        KeyTransformValue(ref keyArray, _valueK, (byte)((_valueK & 0xFF) & 0x03));
        Span<byte> keyBytes = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(keyBytes, keyArray);
        _blowfish.Initialize(keyBytes);

        Span<byte> tmp = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(tmp, _clientKey);
        _blowfish.DecodeInPlace(tmp);
        _clientKey = BinaryPrimitives.ReadUInt64LittleEndian(tmp);

        ulong expected = MakeLongLong(_valueB, _valueA);
        KeyTransformValue(ref expected, _valueK, (byte)((_valueB & 0xFF) & 0x07));
        if (_clientKey != expected)
            throw new SecurityException("[Security::Handshake] Client signature error.");

        // Build challenge response
        keyArray = MakeLongLong(_valueA, _valueB);
        KeyTransformValue(ref keyArray, _valueK, (byte)((_valueK & 0xFF) & 0x03));
        BinaryPrimitives.WriteUInt64LittleEndian(keyBytes, keyArray);
        _blowfish.Initialize(keyBytes);

        _challengeKey = MakeLongLong(_valueA, _valueB);
        KeyTransformValue(ref _challengeKey, _valueK, (byte)((_valueA & 0xFF) & 0x07));
        BinaryPrimitives.WriteUInt64LittleEndian(tmp, _challengeKey);
        _blowfish.EncodeInPlace(tmp);
        _challengeKey = BinaryPrimitives.ReadUInt64LittleEndian(tmp);

        KeyTransformValue(ref _handshakeBlowfishKey, _valueK, 0x3);
        BinaryPrimitives.WriteUInt64LittleEndian(keyBytes, _handshakeBlowfishKey);
        _blowfish.Initialize(keyBytes);

        using var response = new PacketWriter();
        response.WriteByte((byte)SecurityFlag.HandshakeResponse);
        response.WriteUInt64(_challengeKey);

        _outgoingPackets.Add(new Packet(0x5000, false, false, response.Span));
    }

    private void ServerHandshake(ushort opcode, ref PacketReader reader)
    {
        if (opcode != 0x5000)
            throw new SecurityException("[Security::Handshake] Expected 0x5000 from client.");

        byte flag = reader.ReadByte();
        SecurityFlag flags = (SecurityFlag)flag;

        if (_securityFlagByte == 0)
        {
            _securityFlagByte = flag;
            _securityFlags = flags;
        }

        if (flags.HasFlag(SecurityFlag.Blowfish))
        {
            _initialBlowfishKey = reader.ReadUInt64();
            Span<byte> kb = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(kb, _initialBlowfishKey);
            _blowfish.Initialize(kb);
        }

        if (flags.HasFlag(SecurityFlag.SecurityBytes))
        {
            _seedCount = reader.ReadUInt32();
            _crcSeed = reader.ReadUInt32();
            SetupCountByte(_seedCount);
        }

        if (flags.HasFlag(SecurityFlag.Handshake))
        {
            _handshakeBlowfishKey = reader.ReadUInt64();
            _valueG = reader.ReadUInt32();
            _valueP = reader.ReadUInt32();
            _valueA = reader.ReadUInt32();

            _valueX = NextUInt32() & 0x7FFFFFFF;
            _valueB = GPowXModP(_valueP, _valueX, _valueG);
            _valueK = GPowXModP(_valueP, _valueX, _valueA);

            Span<byte> kb = stackalloc byte[8];
            Span<byte> tmp = stackalloc byte[8];

            ulong keyArray = MakeLongLong(_valueA, _valueB);
            KeyTransformValue(ref keyArray, _valueK, (byte)((_valueK & 0xFF) & 0x03));
            BinaryPrimitives.WriteUInt64LittleEndian(kb, keyArray);
            _blowfish.Initialize(kb);

            _clientKey = MakeLongLong(_valueB, _valueA);
            KeyTransformValue(ref _clientKey, _valueK, (byte)((_valueB & 0xFF) & 0x07));
            BinaryPrimitives.WriteUInt64LittleEndian(tmp, _clientKey);
            _blowfish.EncodeInPlace(tmp);
            _clientKey = BinaryPrimitives.ReadUInt64LittleEndian(tmp);
        }

        if (flags.HasFlag(SecurityFlag.HandshakeResponse))
        {
            _challengeKey = reader.ReadUInt64();

            ulong expected = MakeLongLong(_valueA, _valueB);
            KeyTransformValue(ref expected, _valueK, (byte)((_valueA & 0xFF) & 0x07));
            Span<byte> tmp = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(tmp, expected);
            _blowfish.EncodeInPlace(tmp);
            expected = BinaryPrimitives.ReadUInt64LittleEndian(tmp);

            if (_challengeKey != expected)
                throw new SecurityException("[Security::Handshake] Server signature error.");

            KeyTransformValue(ref _handshakeBlowfishKey, _valueK, 0x3);
            Span<byte> kb = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(kb, _handshakeBlowfishKey);
            _blowfish.Initialize(kb);
        }

        if (flags.HasFlag(SecurityFlag.Handshake) && !flags.HasFlag(SecurityFlag.HandshakeResponse))
        {
            if (_startedHandshake || _acceptedHandshake)
                throw new SecurityException("[Security::Handshake] Duplicate 0x5000.");

            using var response = new PacketWriter();
            response.WriteUInt32(_valueB);
            response.WriteUInt64(_clientKey);

            _outgoingPackets.Insert(0, new Packet(0x5000, false, false, response.Span));
            _startedHandshake = true;
        }
        else
        {
            if (_acceptedHandshake)
                throw new SecurityException("[Security::Handshake] Duplicate handshake acceptance.");

            var response1 = new Packet(0x9000);

            var response2 = new Packet(0x2001, true, false);
            response2.WriteAscii(_identityName);
            response2.WriteUInt8(_identityFlag);

            _outgoingPackets.Insert(0, response2);
            _outgoingPackets.Insert(0, response1);

            _startedHandshake = true;
            _acceptedHandshake = true;
        }
    }

    // ---------------------------------------------------------------------
    // Packet formatting
    // ---------------------------------------------------------------------

    private byte[] FormatPacket(ushort opcode, ReadOnlySpan<byte> data, bool encrypted)
    {
        if (data.Length >= 0x8000)
            throw new SecurityException("[Security::FormatPacket] Payload too large.");

        ushort dataLength = (ushort)data.Length;

        using var writer = new PacketWriter(data.Length + 6);
        writer.WriteUInt16(dataLength);
        writer.WriteUInt16(opcode);
        writer.WriteUInt16(0);
        writer.Write(data);

        bool markEncrypted = encrypted &&
            (_securityFlags.HasFlag(SecurityFlag.Blowfish) ||
             (_securityFlags.HasFlag(SecurityFlag.SecurityBytes) &&
              !_securityFlags.HasFlag(SecurityFlag.Blowfish)));

        if (markEncrypted)
            writer.PatchUInt16(0, (ushort)(dataLength | 0x8000));

        if (!_clientSecurity && _securityFlags.HasFlag(SecurityFlag.SecurityBytes))
        {
            byte sb1 = GenerateCountByte(true);
            writer.PatchByte(4, sb1);

            byte sb2 = GenerateCheckByte(writer.Span);
            writer.PatchByte(5, sb2);
        }

        if (encrypted && _securityFlags.HasFlag(SecurityFlag.Blowfish))
        {
            writer.PadSoSliceIsAligned(sliceStart: 2, alignment: 8);
            _blowfish.EncodeInPlace(writer.WritableSpan[2..]);
            return writer.ToArray();
        }

        if (encrypted &&
            _securityFlags.HasFlag(SecurityFlag.SecurityBytes) &&
            !_securityFlags.HasFlag(SecurityFlag.Blowfish))
        {
            writer.PatchUInt16(0, dataLength);
        }

        return writer.ToArray();
    }

    private bool HasPacketToSend()
    {
        if (_outgoingPackets.Count == 0) return false;
        if (_acceptedHandshake) return true;

        var first = _outgoingPackets[0];
        return first.Opcode is 0x5000 or 0x9000;
    }

    private KeyValuePair<TransferBuffer, Packet> GetPacketToSend()
    {
        if (_outgoingPackets.Count == 0)
            throw new SecurityException("[Security::GetPacketToSend] No packets available.");

        var packet = _outgoingPackets[0];
        _outgoingPackets.RemoveAt(0);

        if (packet.Massive)
            return BuildMassivePacket(packet);

        bool encrypted = packet.Encrypted;
        if (!_clientSecurity && _encryptedOpcodes.Contains(packet.Opcode))
            encrypted = true;

        byte[] raw = FormatPacket(packet.Opcode, packet.PayloadSpan, encrypted);
        packet.Lock();
        return new KeyValuePair<TransferBuffer, Packet>(
            new TransferBuffer(raw, 0, raw.Length, assign: true), packet);
    }

    private KeyValuePair<TransferBuffer, Packet> BuildMassivePacket(Packet packet)
    {
        const int MaxChunk = 4089;

        byte[] input = packet.GetBytes();
        ushort parts = 0;

        using var finalData = new PacketWriter(input.Length + 16);
        int offset = 0;
        while (offset < input.Length)
        {
            int cur = Math.Min(MaxChunk, input.Length - offset);

            using var part = new PacketWriter(cur + 1);
            part.WriteByte(0); // data flag
            part.Write(input.AsSpan(offset, cur));

            byte[] formatted = FormatPacket(0x600D, part.Span, encrypted: false);
            finalData.Write(formatted);

            offset += cur;
            parts++;
        }

        using var header = new PacketWriter(5);
        header.WriteByte(1);
        header.WriteUInt16(parts);
        header.WriteUInt16(packet.Opcode);

        using var final = new PacketWriter(input.Length + 32);
        final.Write(FormatPacket(0x600D, header.Span, encrypted: false));
        final.Write(finalData.Span);

        byte[] raw = final.ToArray();
        packet.Lock();
        return new KeyValuePair<TransferBuffer, Packet>(
            new TransferBuffer(raw, 0, raw.Length, assign: true), packet);
    }

    // ---------------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------------

    public void Send(Packet packet)
    {
        ArgumentNullException.ThrowIfNull(packet);
        if (packet.Opcode is 0x5000 or 0x9000)
            throw new SecurityException("[Security::Send] Handshake packets cannot be sent here.");
        _outgoingPackets.Add(packet);
    }

    public void Recv(byte[] buffer, int offset, int length)
        => Recv(new TransferBuffer(buffer, offset, length, assign: true));

    public void Recv(TransferBuffer rawBuffer)
    {
        ArgumentNullException.ThrowIfNull(rawBuffer);

        int available = rawBuffer.Size - rawBuffer.Offset;
        int srcIndex = rawBuffer.Offset;

        while (available > 0)
        {
            // --- If we don't have a current packet, acquire the 2-byte length
            //     prefix and allocate _currentBuffer. This block must ONLY run
            //     when _currentBuffer is null — otherwise bytes belonging to
            //     the in-progress packet would be misrouted into _recvBuffer
            //     and mistaken for the next packet's length prefix.
            if (_currentBuffer is null)
            {
                if (_recvBuffer.Size < 2)
                {
                    int need = 2 - _recvBuffer.Size;
                    int take = Math.Min(need, available);
                    Buffer.BlockCopy(rawBuffer.Buffer, srcIndex,
                                     _recvBuffer.Buffer, _recvBuffer.Size, take);
                    _recvBuffer.Size += take;
                    srcIndex += take;
                    available -= take;
                    if (_recvBuffer.Size < 2)
                        return; // need more data to know the length
                }

                int payload = _recvBuffer.Buffer[1] << 8 | _recvBuffer.Buffer[0];
                int wireSize;
                if ((payload & 0x8000) > 0)
                {
                    payload &= 0x7FFF;
                    wireSize = _securityFlags.HasFlag(SecurityFlag.Blowfish)
                        ? 2 + Blowfish.GetOutputLength(payload + 4)
                        : payload + 6;
                }
                else
                {
                    wireSize = payload + 6;
                }

                _currentBuffer = new TransferBuffer(wireSize, 0, wireSize);

                // Move the 2-byte prefix into the new packet buffer.
                int move = Math.Min(_recvBuffer.Size, wireSize);
                Buffer.BlockCopy(_recvBuffer.Buffer, 0,
                                 _currentBuffer.Buffer, 0, move);
                _currentBuffer.Offset = move;

                // Slide any surplus (should normally be zero; defensive).
                if (_recvBuffer.Size > move)
                {
                    int surplus = _recvBuffer.Size - move;
                    Buffer.BlockCopy(_recvBuffer.Buffer, move,
                                     _recvBuffer.Buffer, 0, surplus);
                    _recvBuffer.Size = surplus;
                }
                else
                {
                    _recvBuffer.Size = 0;
                }
            }

            // --- Fill _currentBuffer directly from the incoming raw buffer.
            int needMore = _currentBuffer.Size - _currentBuffer.Offset;
            if (needMore > 0)
            {
                int take = Math.Min(needMore, available);
                Buffer.BlockCopy(rawBuffer.Buffer, srcIndex,
                                 _currentBuffer.Buffer, _currentBuffer.Offset, take);
                _currentBuffer.Offset += take;
                srcIndex += take;
                available -= take;
            }

            if (_currentBuffer.Size == _currentBuffer.Offset)
            {
                var done = _currentBuffer;
                done.Offset = 0;
                _currentBuffer = null;
                ProcessIncomingBuffer(done);
            }
            else
            {
                return; // need more data to finish the current packet
            }
        }
    }

    private void ProcessIncomingBuffer(TransferBuffer buffer)
    {
        if (buffer.Size < 6 || buffer.Size > buffer.Buffer.Length)
            throw new SecurityException(
                $"[Security::Recv] Invalid buffer state: Size={buffer.Size}, Len={buffer.Buffer.Length}.");

        Span<byte> span = buffer.Buffer.AsSpan(0, buffer.Size);

        bool packetEncrypted = false;
        int packetSize = BinaryPrimitives.ReadUInt16LittleEndian(span);

        if ((packetSize & 0x8000) > 0)
        {
            packetSize &= 0x7FFF;
            if (_securityFlags.HasFlag(SecurityFlag.Blowfish))
                packetEncrypted = true;
        }

        if (packetEncrypted)
        {
            Span<byte> cipher = buffer.Buffer.AsSpan(2, buffer.Size - 2);
            if ((cipher.Length & 7) != 0)
                throw new SecurityException(
                    $"[Security::Recv] Encrypted payload length {cipher.Length} is not a multiple of 8.");

            _blowfish.DecodeInPlace(cipher);

            BinaryPrimitives.WriteUInt16LittleEndian(buffer.Buffer, (ushort)packetSize);
            span = buffer.Buffer.AsSpan(0, 6 + packetSize);
        }

        var reader = new PacketReader(buffer.Buffer.AsMemory(0, buffer.Size));
        _ = reader.ReadUInt16(); // size (already parsed)
        ushort opcode = reader.ReadUInt16();
        byte securityCount = reader.ReadByte();
        byte securityCrc = reader.ReadByte();

        if (_clientSecurity && _securityFlags.HasFlag(SecurityFlag.SecurityBytes))
        {
            byte expectedCount = GenerateCountByte(true);
            if (securityCount != expectedCount)
                throw new SecurityException("[Security::Recv] Count byte mismatch.");

            if (packetEncrypted || !_securityFlags.HasFlag(SecurityFlag.Blowfish))
            {
                if (packetEncrypted || _encryptedOpcodes.Contains(opcode))
                {
                    ushort marked = (ushort)(packetSize | 0x8000);
                    BinaryPrimitives.WriteUInt16LittleEndian(span, marked);
                }
            }

            // Zero out the CRC byte before recomputing.
            span[5] = 0;
            byte expectedCrc = GenerateCheckByte(span);
            if (securityCrc != expectedCrc)
                throw new SecurityException("[Security::Recv] CRC byte mismatch.");
            span[4] = 0;

            if (packetEncrypted || !_securityFlags.HasFlag(SecurityFlag.Blowfish))
            {
                if (packetEncrypted || _encryptedOpcodes.Contains(opcode))
                {
                    BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort)packetSize);
                }
            }
        }

        if (opcode is 0x5000 or 0x9000)
        {
            Handshake(opcode, ref reader, packetEncrypted);
            var hsPacket = new Packet(opcode, packetEncrypted, false,
                                      buffer.Buffer, 6, packetSize);
            hsPacket.Lock();
            _incomingPackets.Add(hsPacket);
            return;
        }

        if (_clientSecurity && !_acceptedHandshake)
            throw new SecurityException("[Security::Recv] Client has not accepted the handshake.");

        if (opcode == 0x600D)
        {
            byte mode = reader.ReadByte();
            if (mode == 1)
            {
                _massiveCount = reader.ReadUInt16();
                ushort contained = reader.ReadUInt16();
                _massivePacket = new Packet(contained, packetEncrypted, true);
            }
            else
            {
                if (_massivePacket is null)
                    throw new SecurityException("[Security::Recv] Malformed 0x600D packet.");

                _massivePacket.WriteUInt8Array(reader.ReadBytes(packetSize - 1));
                _massiveCount--;
                if (_massiveCount == 0)
                {
                    _massivePacket.Lock();
                    _incomingPackets.Add(_massivePacket);
                    _massivePacket = null;
                }
            }
            return;
        }

        var pkt = new Packet(opcode, packetEncrypted, false, buffer.Buffer, 6, packetSize);
        pkt.Lock();
        _incomingPackets.Add(pkt);
    }

    public List<KeyValuePair<TransferBuffer, Packet>> TransferOutgoing()
    {
        var buffers = new List<KeyValuePair<TransferBuffer, Packet>>();
        while (HasPacketToSend())
            buffers.Add(GetPacketToSend());
        return buffers;
    }

    public IReadOnlyList<Packet> TransferIncoming()
    {
        if (_incomingPackets.Count == 0) return [];
        var packets = _incomingPackets;
        _incomingPackets = [];
        return packets;
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------

    private static ulong MakeLongLong(uint a, uint b)
        => ((ulong)b << 32) | a;
}