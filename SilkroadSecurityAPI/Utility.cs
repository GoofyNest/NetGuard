using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace SilkroadSecurityAPI;

public static class Utility
{
    // Hex digit lookup — one table read per nibble, no format-string parsing.
    private const string HexDigits = "0123456789ABCDEF";

    /// <summary>
    /// Formats a byte range as a classic hex + ASCII dump.
    /// Output is one line per 16 bytes: "OFFSET  HH HH ... HH  |ascii|".
    /// </summary>
    public static string HexDump(ReadOnlySpan<byte> buffer)
        => HexDump(buffer, 0, buffer.Length);

    public static string HexDump(ReadOnlySpan<byte> buffer, int offset, int count = -1)
    {
        if (buffer.IsEmpty) return string.Empty;

        if (offset < 0 || offset > buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));

        int available = buffer.Length - offset;
        int length = count < 0 ? available : Math.Min(count, available);
        if (length <= 0) return string.Empty;

        const int bytesPerLine = 16;

        // Each full line: "XXXXXXXX  " (10) + 16*3 hex (48) + "  " (2) + 16 ascii + "\n" (1) = 77 chars.
        // Pre-size generously; StringBuilder will grow once at most.
        int estimated = ((length + bytesPerLine - 1) / bytesPerLine) * 80;
        var sb = new StringBuilder(estimated);

        int lines = (length + bytesPerLine - 1) / bytesPerLine;
        char[]? rentedAscii = null;
        Span<char> ascii = bytesPerLine <= 64
            ? stackalloc char[bytesPerLine]
            : (rentedAscii = ArrayPool<char>.Shared.Rent(bytesPerLine));

        try
        {
            for (int line = 0; line < lines; line++)
            {
                int lineStart = line * bytesPerLine;
                int lineLen = Math.Min(bytesPerLine, length - lineStart);
                int absolute = offset + lineStart;

                // Offset column: 8 hex digits, no allocations.
                AppendHex32(sb, (uint)absolute);
                sb.Append("  ");

                // Hex columns.
                for (int i = 0; i < lineLen; i++)
                {
                    byte b = buffer[offset + lineStart + i];
                    sb.Append(HexDigits[b >> 4]);
                    sb.Append(HexDigits[b & 0x0F]);
                    sb.Append(' ');
                    ascii[i] = b is >= 32 and <= 126 ? (char)b : '.';
                }

                // Right-pad short lines so the ASCII column lines up.
                if (lineLen < bytesPerLine)
                    sb.Append(' ', (bytesPerLine - lineLen) * 3);

                sb.Append("  ");
                sb.Append(ascii[..lineLen]);
                sb.Append('\n');
            }
        }
        finally
        {
            if (rentedAscii is not null)
                ArrayPool<char>.Shared.Return(rentedAscii);
        }

        // Trim the trailing newline to match the old behavior (single block, no trailing \n).
        if (sb.Length > 0 && sb[^1] == '\n')
            sb.Length--;

        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendHex32(StringBuilder sb, uint value)
    {
        // 8 hex digits, big-endian, no format-string parsing.
        sb.Append(HexDigits[(int)((value >> 28) & 0xF)]);
        sb.Append(HexDigits[(int)((value >> 24) & 0xF)]);
        sb.Append(HexDigits[(int)((value >> 20) & 0xF)]);
        sb.Append(HexDigits[(int)((value >> 16) & 0xF)]);
        sb.Append(HexDigits[(int)((value >> 12) & 0xF)]);
        sb.Append(HexDigits[(int)((value >> 8) & 0xF)]);
        sb.Append(HexDigits[(int)((value >> 4) & 0xF)]);
        sb.Append(HexDigits[(int)(value & 0xF)]);
    }

    /// <summary>Formats bytes as a compact contiguous hex string ("0D00476174...").</summary>
    public static string ToHexString(ReadOnlySpan<byte> buffer)
    {
        if (buffer.IsEmpty) return string.Empty;

        char[] rented = ArrayPool<char>.Shared.Rent(buffer.Length * 2);
        try
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                byte b = buffer[i];
                rented[i * 2] = HexDigits[b >> 4];
                rented[i * 2 + 1] = HexDigits[b & 0x0F];
            }
            return new string(rented, 0, buffer.Length * 2);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    /// <summary>Renders a single line: "0000  0D 00 47 61 ...   |..Ga|".</summary>
    public static string HexLine(ReadOnlySpan<byte> buffer, int offset = 0)
    {
        if (buffer.IsEmpty) return string.Empty;

        int length = Math.Min(buffer.Length - offset, 16);
        if (length <= 0) return string.Empty;

        Span<char> line = stackalloc char[80];
        int p = 0;

        AppendHex16(line, ref p, (ushort)offset);
        line[p++] = ' '; line[p++] = ' ';

        for (int i = 0; i < length; i++)
        {
            byte b = buffer[offset + i];
            line[p++] = HexDigits[b >> 4];
            line[p++] = HexDigits[b & 0x0F];
            line[p++] = ' ';
        }
        for (int i = length; i < 16; i++)
        {
            line[p++] = ' '; line[p++] = ' '; line[p++] = ' ';
        }
        line[p++] = ' '; line[p++] = '|';
        for (int i = 0; i < length; i++)
        {
            byte b = buffer[offset + i];
            line[p++] = b is >= 32 and <= 126 ? (char)b : '.';
        }
        line[p++] = '|';

        return new string(line[..p]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendHex16(Span<char> dst, ref int pos, ushort value)
    {
        dst[pos++] = HexDigits[(value >> 12) & 0xF];
        dst[pos++] = HexDigits[(value >> 8) & 0xF];
        dst[pos++] = HexDigits[(value >> 4) & 0xF];
        dst[pos++] = HexDigits[value & 0xF];
    }

    /// <summary>Parses a hex string into bytes. Accepts whitespace and optional 0x prefixes.</summary>
    public static byte[] FromHexString(ReadOnlySpan<char> hex)
    {
        if (hex.IsEmpty) return [];

        int nibbleCount = 0;
        foreach (char c in hex)
            if (IsHexDigit(c)) nibbleCount++;

        if ((nibbleCount & 1) != 0)
            throw new FormatException("Hex string has an odd number of digits.");

        byte[] result = new byte[nibbleCount / 2];
        int outIdx = 0;
        int high = -1;

        foreach (char c in hex)
        {
            if (!IsHexDigit(c)) continue;
            int nib = HexValue(c);
            if (high < 0) high = nib;
            else
            {
                result[outIdx++] = (byte)((high << 4) | nib);
                high = -1;
            }
        }
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsHexDigit(char c) =>
        (c >= '0' && c <= '9') ||
        (c >= 'a' && c <= 'f') ||
        (c >= 'A' && c <= 'F');

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int HexValue(char c) =>
        c <= '9' ? c - '0' :
        c <= 'F' ? c - 'A' + 10 :
                   c - 'a' + 10;
}