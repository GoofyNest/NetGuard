using System.Text;
using NetGuard.DesktopProtocol;

namespace NetGuard.Desktop;

internal static class PacketFormatter
{
    internal static string Format(DesktopMessage packet)
    {
        var text = new StringBuilder();
        text.AppendLine($"[{packet.Time:HH:mm:ss.fff}] module={packet.Module} IP={packet.Ip} client={packet.ClientId}");
        text.AppendLine($"[{(packet.FromClient ? "C -> S" : "S -> C")}] [0x{packet.Opcode:X4}] {packet.Length} bytes   encrypted={packet.Encrypted} massive={packet.Massive}");
        text.AppendLine(packet.Stage == "injected" ? "Injected through the ordered socket writer — payload offsets start at 0000."
            : "Received before filters — payload offsets start at 0000.");
        byte[] bytes = packet.Payload ?? [];
        for (int offset = 0; offset < bytes.Length; offset += 16)
        {
            int count = Math.Min(16, bytes.Length - offset);
            text.Append($"{offset:X4}  ");
            for (int column = 0; column < 16; column++) text.Append(column < count ? $"{bytes[offset + column]:X2} " : "   ");
            text.Append("  ");
            for (int column = 0; column < count; column++)
            {
                byte value = bytes[offset + column];
                text.Append(value is >= 32 and <= 126 ? (char)value : '.');
            }
            text.AppendLine();
        }
        if (bytes.Length < packet.Length) text.AppendLine($"[Capture truncated: first {bytes.Length:N0} of {packet.Length:N0} bytes]");
        return text.ToString();
    }
}
