using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Framework
{
    public static class ByteArrayExtension
    {
        public static Type ToStruct<Type>(this byte[] buffer)
        {
            IntPtr pointer = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, pointer, buffer.Length);
            var obj = (Type)Marshal.PtrToStructure(pointer, default(Type).GetType());
            Marshal.FreeHGlobal(pointer);
            return obj;
        }
        public static string HexDump(this byte[] buffer)
        {
            return HexDump(buffer, 0, buffer.Length);
        }
        public static string HexDump(this byte[] buffer, int offset, int count)
        {
            const int bytesPerLine = 16;
            StringBuilder output = new StringBuilder();
            StringBuilder ascii_output = new StringBuilder();
            int length = count;
            if (length % bytesPerLine != 0)
            {
                length += bytesPerLine - length % bytesPerLine;
            }
            for (int x = 0; x <= length; ++x)
            {
                if (x % bytesPerLine == 0)
                {
                    if (x > 0)
                    {
                        output.AppendFormat("  {0}{1}", ascii_output.ToString(), Environment.NewLine);
                        ascii_output.Clear();
                    }
                    if (x != length)
                    {
                        output.AppendFormat("{0:d10}   ", x);
                    }
                }
                if (x < count)
                {
                    output.AppendFormat(buffer[offset + x].ToString("X2") + " ");
                    char ch = (char)buffer[offset + x];
                    ascii_output.Append(char.IsControl(ch) ? '.' : ch);
                }
                else
                {
                    output.Append("   ");
                    ascii_output.Append('.');
                }
            }
            return output.ToString();
        }
    }
    public static class ObjectExtension
    {
        public static byte[] ToBuffer(this object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] retval = new byte[size];
            IntPtr pointer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, pointer, true);
            Marshal.Copy(pointer, retval, 0, size);
            Marshal.FreeHGlobal(pointer);
            return retval;
        }
    }
}