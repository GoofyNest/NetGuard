using System;
using System.Text;

namespace SilkroadSecurityAPI
{
    public class Utility
    {
        private const int bytesPerLine = 16;

        public static string HexDump(byte[] buffer)
        {
            return HexDump(buffer, 0, buffer.Length);
        }

        public static string HexDump(byte[] buffer, int offset, int count)
        {
            int fullLines = count / bytesPerLine;
            int remainingBytes = count % bytesPerLine;

            StringBuilder output = new StringBuilder(count * 3); // Optimized size for the StringBuilder
            StringBuilder ascii_output = new StringBuilder(bytesPerLine);

            // Iterate through the data and format it
            for (int i = 0; i < fullLines; ++i)
            {
                int lineStart = offset + i * bytesPerLine;
                AppendHexLine(buffer, lineStart, bytesPerLine, output, ascii_output);
            }

            // Handle remaining bytes
            if (remainingBytes > 0)
            {
                int lineStart = offset + fullLines * bytesPerLine;
                AppendHexLine(buffer, lineStart, remainingBytes, output, ascii_output, true);
            }

            return output.ToString();
        }

        private static void AppendHexLine(byte[] buffer, int lineStart, int byteCount, StringBuilder hexOutput, StringBuilder asciiOutput, bool pad = false)
        {
            hexOutput.AppendFormat("{0:d10}   ", lineStart);
            for (int i = 0; i < byteCount; ++i)
            {
                hexOutput.AppendFormat("{0:X2} ", buffer[lineStart + i]);

                // Append ASCII character if it's printable, otherwise append a "."
                char ch = (char)buffer[lineStart + i];
                asciiOutput.Append(Char.IsControl(ch) ? '.' : ch);
            }

            // Pad remaining space if required
            if (pad)
            {
                hexOutput.Append(new string(' ', (bytesPerLine - byteCount) * 3)); // Pad for the remaining hex values
                asciiOutput.Append(new string('.', bytesPerLine - byteCount));
            }

            // Append the ASCII part
            hexOutput.Append("  ");
            hexOutput.Append(asciiOutput.ToString());
            hexOutput.AppendLine();
            asciiOutput.Clear();
        }
    }
}