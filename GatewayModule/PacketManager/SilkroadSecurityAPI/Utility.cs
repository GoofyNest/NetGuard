using System.Text;

namespace GatewayModule.SilkroadSecurityAPI
{
    public class Utility
    {
        public static string HexDump(byte[] buffer)
        {
            return HexDump(buffer, 0, buffer.Length);
        }

        public static string HexDump(byte[] buffer, int offset = 0, int count = -1)
        {
            if (buffer == null) 
                return string.Empty;

            const int bytesPerLine = 16;
            int length = count < 0 ? buffer.Length - offset : count;
            if (length <= 0 || offset >= buffer.Length) 
                return string.Empty;

            var output = new StringBuilder(length * 4); // Pre-size for efficiency
            var asciiOutput = new char[bytesPerLine];

            for (int i = 0; i < length; i++)
            {
                if (i % bytesPerLine == 0)
                {
                    if (i > 0) output.Append("  ").Append(asciiOutput).AppendLine();
                    output.AppendFormat("{0:X8}  ", i + offset);
                }

                byte b = buffer[offset + i];
                output.AppendFormat("{0:X2} ", b);
                asciiOutput[i % bytesPerLine] = (b >= 32 && b <= 126) ? (char)b : '.';
            }

            int remainingBytes = length % bytesPerLine;
            if (remainingBytes > 0)
            {
                int padding = (bytesPerLine - remainingBytes) * 3;
                output.Append(' ', padding).Append("  ");
                output.Append(asciiOutput, 0, remainingBytes);
            }

            return output.ToString();
        }
    }
}