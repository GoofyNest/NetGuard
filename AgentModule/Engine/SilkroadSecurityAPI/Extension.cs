using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SilkroadSecurityAPI
{
    public static class ByteArrayExtension
    {
        /// <summary>
        /// Converts a byte array to a struct of the specified type.
        /// </summary>
        /// <typeparam name="T">The struct type to convert to.</typeparam>
        /// <param name="buffer">The byte array containing the data.</param>
        /// <returns>The converted struct.</returns>
        public static T ToStruct<T>(this byte[] buffer) where T : struct
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length != Marshal.SizeOf<T>()) throw new ArgumentException("Buffer size does not match the size of the struct.");

            IntPtr pointer = Marshal.AllocHGlobal(buffer.Length);
            try
            {
                Marshal.Copy(buffer, 0, pointer, buffer.Length);
                return Marshal.PtrToStructure<T>(pointer);
            }
            finally
            {
                Marshal.FreeHGlobal(pointer);
            }
        }

        /// <summary>
        /// Generates a hexadecimal representation of the byte array.
        /// </summary>
        /// <param name="buffer">The byte array to dump.</param>
        /// <returns>The hexadecimal dump string.</returns>
        public static string HexDump(this byte[] buffer) => buffer.HexDump(0, buffer?.Length ?? 0);

        /// <summary>
        /// Generates a hexadecimal representation of a portion of the byte array.
        /// </summary>
        /// <param name="buffer">The byte array to dump.</param>
        /// <param name="offset">The starting offset in the array.</param>
        /// <param name="count">The number of bytes to include.</param>
        /// <returns>The hexadecimal dump string.</returns>
        public static string HexDump(this byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("Invalid offset or count.");

            const int bytesPerLine = 16;
            var output = new StringBuilder();
            var asciiOutput = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                if (i % bytesPerLine == 0)
                {
                    if (i > 0)
                    {
                        output.AppendFormat("  {0}{1}", asciiOutput, Environment.NewLine);
                        asciiOutput.Clear();
                    }
                    output.AppendFormat("{0:D8}: ", offset + i);
                }

                byte b = buffer[offset + i];
                output.AppendFormat("{0:X2} ", b);
                asciiOutput.Append(char.IsControl((char)b) ? '.' : (char)b);
            }

            int remaining = bytesPerLine - count % bytesPerLine;
            if (remaining < bytesPerLine)
            {
                output.Append(new string(' ', remaining * 3));
                output.AppendFormat("  {0}{1}", asciiOutput, Environment.NewLine);
            }

            return output.ToString();
        }
    }

    public static class ObjectExtension
    {
        /// <summary>
        /// Converts a struct to a byte array.
        /// </summary>
        /// <param name="obj">The struct to convert.</param>
        /// <returns>A byte array representing the struct.</returns>
        public static byte[] ToBuffer(this object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            int size = Marshal.SizeOf(obj);
            byte[] buffer = new byte[size];
            IntPtr pointer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(obj, pointer, true);
                Marshal.Copy(pointer, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(pointer);
            }
            return buffer;
        }
    }
}