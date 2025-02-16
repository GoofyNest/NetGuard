using System;
using System.Text;
using System.IO;

namespace SilkroadSecurityAPI
{
    public class Packet
    {
        private bool Locked { get; set; }
        private byte[] Reader_bytes { get; set; }
        private PacketWriter m_writer;
        private PacketReader m_reader;

        public ushort Opcode { get; }

        public bool Encrypted { get; }

        public bool Massive { get; }

        public Packet(Packet rhs)
        {
            Opcode = rhs.Opcode;
            Encrypted = rhs.Encrypted;
            Massive = rhs.Massive;

            Locked = rhs.Locked;
            if (!Locked)
            {
                m_writer = new PacketWriter();
                m_reader = null!;
                Reader_bytes = null!;
                m_writer.Write(rhs.m_writer.GetBytes());
            }
            else
            {
                m_writer = null!;
                Reader_bytes = rhs.Reader_bytes;
                m_reader = new PacketReader(Reader_bytes);
            }
        }

        public Packet(ushort opcode)
        {
            Opcode = opcode;
            Encrypted = false;
            Massive = false;
            m_writer = new PacketWriter();
            m_reader = null!;
            Reader_bytes = null!;
        }

        public Packet(ushort opcode, bool encrypted)
        {
            Opcode = opcode;
            Encrypted = encrypted;
            Massive = false;
            m_writer = new PacketWriter();
            m_reader = null!;
            Reader_bytes = null!;
        }

        public Packet(ushort opcode, bool encrypted, bool massive)
        {
            if (encrypted && massive)
                throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");

            Opcode = opcode;
            Encrypted = encrypted;
            Massive = massive;
            m_writer = new PacketWriter();
            m_reader = null!;
            Reader_bytes = null!;
        }

        public Packet(ushort opcode, bool encrypted, bool massive, byte[] bytes)
        {
            if (encrypted && massive)
                throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");

            Opcode = opcode;
            Encrypted = encrypted;
            Massive = massive;
            m_writer = new PacketWriter();
            m_writer.Write(bytes);
            m_reader = null!;
            Reader_bytes = null!;
        }

        public Packet(ushort opcode, bool encrypted, bool massive, byte[] bytes, int offset, int length)
        {
            if (encrypted && massive)
                throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");

            Opcode = opcode;
            Encrypted = encrypted;
            Massive = massive;
            m_writer = new PacketWriter();
            m_writer.Write(bytes, offset, length);
            m_reader = null!;
            Reader_bytes = null!;
        }

        public byte[] GetBytes()
        {
            if (Locked)
                return Reader_bytes;

            return m_writer.GetBytes();
        }

        public void Lock()
        {
            if (!Locked)
            {
                Reader_bytes = m_writer.GetBytes();
                m_reader = new PacketReader(Reader_bytes);
                m_writer.Close();
                m_writer = null!;
                Locked = true;
            }
        }

        public long SeekRead(long offset, SeekOrigin orgin)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.BaseStream.Seek(offset, orgin);
        }

        public int RemainingRead()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return (int)(m_reader.BaseStream.Length - m_reader.BaseStream.Position);
        }

        public byte ReadUInt8()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadByte();
        }

        public sbyte ReadInt8()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadSByte();
        }

        public UInt16 ReadUInt16()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadUInt16();
        }

        public Int16 ReadInt16()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadInt16();
        }

        public UInt32 ReadUInt32()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadUInt32();
        }

        public Int32 ReadInt32()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadInt32();
        }

        public UInt64 ReadUInt64()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadUInt64();
        }

        public Int64 ReadInt64()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadInt64();
        }

        public Single ReadSingle()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadSingle();
        }

        public Double ReadDouble()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            return m_reader.ReadDouble();
        }

        public String ReadAscii()
        {
            return ReadAscii(1252);
        }

        public string ReadAscii(int codepage)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            // Read the length of the string
            ushort length = m_reader.ReadUInt16();

            // Read the actual bytes from the stream
            byte[] bytes = m_reader.ReadBytes(length);

            // Get the encoding based on the provided codepage
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(codepage);

            // If encoding is not found, return an empty string (or handle accordingly)
            if (encoding == null)
                return string.Empty;  // You can choose to throw an exception if needed

            // Use the encoding to convert bytes to a string
            string result = encoding.GetString(bytes);

            // Return the result string
            return result;
        }

        public String ReadUnicode()
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            UInt16 length = m_reader.ReadUInt16();
            byte[] bytes = m_reader.ReadBytes(length * 2);

            return Encoding.Unicode.GetString(bytes);
        }

        public byte[] ReadUInt8Array(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            byte[] values = new byte[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadByte();

            return values;

        }

        public sbyte[] ReadInt8Array(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            sbyte[] values = new sbyte[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadSByte();

            return values;
        }

        public UInt16[] ReadUInt16Array(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            UInt16[] values = new UInt16[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadUInt16();

            return values;
        }

        public Int16[] ReadInt16Array(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            Int16[] values = new Int16[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadInt16();

            return values;
        }

        public UInt32[] ReadUInt32Array(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            UInt32[] values = new UInt32[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadUInt32();

            return values;
        }

        public Int32[] ReadInt32Array(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            Int32[] values = new Int32[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadInt32();

            return values;
        }

        public UInt64[] ReadUInt64Array(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            UInt64[] values = new UInt64[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadUInt64();

            return values;

        }

        public Int64[] ReadInt64Array(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            Int64[] values = new Int64[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadInt64();

            return values;
        }

        public Single[] ReadSingleArray(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            Single[] values = new Single[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadSingle();

            return values;
        }

        public Double[] ReadDoubleArray(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            Double[] values = new Double[count];

            for (int x = 0; x < count; ++x)
                values[x] = m_reader.ReadDouble();

            return values;
        }

        public String[] ReadUnicodeArray(int count)
        {
            if (!Locked)
                throw new Exception("Cannot Read from an unlocked Packet.");

            String[] values = new String[count];

            for (int x = 0; x < count; ++x)
            {
                UInt16 length = m_reader.ReadUInt16();
                byte[] bytes = m_reader.ReadBytes(length * 2);
                values[x] = Encoding.Unicode.GetString(bytes);
            }

            return values;
        }

        public long SeekWrite(long offset, SeekOrigin orgin)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            return m_writer.BaseStream.Seek(offset, orgin);
        }

        public void WriteUInt8(byte value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteInt8(sbyte value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteUInt16(UInt16 value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteInt16(Int16 value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteUInt32(UInt32 value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteInt32(Int32 value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteUInt64(UInt64 value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");
            m_writer.Write(value);
        }

        public void WriteInt64(Int64 value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteSingle(Single value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteDouble(Double value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(value);
        }

        public void WriteAscii(String value)
        {
            WriteAscii(value, 1252);
        }

        public void WriteAscii(string value, int code_page)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            // Ensure the encoding is found based on the provided code_page
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(code_page) ?? throw new Exception($"Encoding with codepage {code_page} not found.");

            // Convert the string to bytes using the encoding
            byte[] codepage_bytes = encoding.GetBytes(value);

            string utf7_value = Encoding.UTF8.GetString(codepage_bytes);

            byte[] bytes = Encoding.Default.GetBytes(utf7_value);

            // Write the length of the byte array and the actual byte array
            m_writer.Write((ushort)bytes.Length);
            m_writer.Write(bytes);
        }

        public void WriteUnicode(String value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            byte[] bytes = Encoding.Unicode.GetBytes(value);

            m_writer.Write((ushort)value.ToString().Length);
            m_writer.Write(bytes);
        }

        public void WriteUInt8(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write((byte)(Convert.ToUInt64(value) & 0xFF));
        }

        public void WriteInt8(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write((sbyte)(Convert.ToInt64(value) & 0xFF));
        }

        public void WriteUInt16(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write((ushort)(Convert.ToUInt64(value) & 0xFFFF));

        }

        public void WriteInt16(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write((ushort)(Convert.ToInt64(value) & 0xFFFF));

        }

        public void WriteUInt32(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write((uint)(Convert.ToUInt64(value) & 0xFFFFFFFF));
        }

        public void WriteInt32(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write((int)(Convert.ToInt64(value) & 0xFFFFFFFF));
        }

        public void WriteUInt64(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(Convert.ToUInt64(value));
        }

        public void WriteInt64(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(Convert.ToInt64(value));
        }

        public void WriteSingle(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(Convert.ToSingle(value));
        }

        public void WriteDouble(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(Convert.ToDouble(value));
        }

        public void WriteAscii(object value)
        {
            WriteAscii(value, 1252);
        }

        public void WriteAscii(object value, int code_page)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            // Ensure value is not null
            string valueAsString = value?.ToString() ?? string.Empty;

            // Get the encoding based on codepage
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(code_page) ?? throw new Exception($"Encoding with codepage {code_page} not found.");

            // Convert string to bytes using the obtained encoding
            byte[] codepage_bytes = encoding.GetBytes(valueAsString);

            // Optionally, if you need UTF-8 conversion, do it here
            byte[] bytes = Encoding.Default.GetBytes(Encoding.UTF8.GetString(codepage_bytes));

            // Write length and bytes
            m_writer.Write((ushort)bytes.Length);
            m_writer.Write(bytes);
        }

        public void WriteUnicode(object value)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            string valueAsString = value?.ToString() ?? string.Empty;

            byte[] bytes = Encoding.Unicode.GetBytes(valueAsString);

            m_writer.Write((ushort)valueAsString.Length);
            m_writer.Write(bytes);
        }

        public void WriteUInt8Array(byte[] values)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            m_writer.Write(values);
        }

        public void WriteUInt8Array(byte[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
                m_writer.Write(values[x]);
        }

        public void WriteUInt16Array(UInt16[] values)
        {
            WriteUInt16Array(values, 0, values.Length);
        }

        public void WriteUInt16Array(UInt16[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
                m_writer.Write(values[x]);
        }
        public void WriteInt16Array(Int16[] values)
        {
            WriteInt16Array(values, 0, values.Length);
        }

        public void WriteInt16Array(Int16[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }

        public void WriteUInt32Array(UInt32[] values)
        {
            WriteUInt32Array(values, 0, values.Length);
        }

        public void WriteUInt32Array(UInt32[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }

        public void WriteInt32Array(Int32[] values)
        {
            WriteInt32Array(values, 0, values.Length);
        }

        public void WriteInt32Array(Int32[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }

        public void WriteUInt64Array(UInt64[] values)
        {
            WriteUInt64Array(values, 0, values.Length);
        }

        public void WriteUInt64Array(UInt64[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }

        public void WriteInt64Array(Int64[] values)
        {
            WriteInt64Array(values, 0, values.Length);
        }

        public void WriteInt64Array(Int64[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }

        public void WriteSingleArray(float[] values)
        {
            WriteSingleArray(values, 0, values.Length);
        }

        public void WriteSingleArray(float[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }

        public void WriteDoubleArray(double[] values)
        {
            WriteDoubleArray(values, 0, values.Length);
        }

        public void WriteDoubleArray(double[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                m_writer.Write(values[x]);
            }
        }

        public void WriteAsciiArray(String[] values, int codepage)
        {
            WriteAsciiArray(values, 0, values.Length, codepage);
        }

        public void WriteAsciiArray(String[] values, int index, int count, int codepage)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteAscii(values[x], codepage);
            }
        }

        public void WriteAsciiArray(String[] values)
        {
            WriteAsciiArray(values, 0, values.Length, 1252);
        }

        public void WriteAsciiArray(String[] values, int index, int count)
        {
            WriteAsciiArray(values, index, count, 1252);
        }

        public void WriteUnicodeArray(String[] values)
        {
            WriteUnicodeArray(values, 0, values.Length);
        }

        public void WriteUnicodeArray(String[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteUnicode(values[x]);
            }
        }

        public void WriteUInt8Array(object[] values)
        {
            WriteUInt8Array(values, 0, values.Length);
        }

        public void WriteUInt8Array(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteUInt8(values[x]);
            }
        }

        public void WriteInt8Array(object[] values)
        {
            WriteInt8Array(values, 0, values.Length);
        }

        public void WriteInt8Array(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteInt8(values[x]);
            }
        }

        public void WriteUInt16Array(object[] values)
        {
            WriteUInt16Array(values, 0, values.Length);
        }

        public void WriteUInt16Array(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteUInt16(values[x]);
            }
        }
        public void WriteInt16Array(object[] values)
        {
            WriteInt16Array(values, 0, values.Length);
        }
        public void WriteInt16Array(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteInt16(values[x]);
            }
        }

        public void WriteUInt32Array(object[] values)
        {
            WriteUInt32Array(values, 0, values.Length);
        }

        public void WriteUInt32Array(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteUInt32(values[x]);
            }
        }

        public void WriteInt32Array(object[] values)
        {
            WriteInt32Array(values, 0, values.Length);
        }

        public void WriteInt32Array(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteInt32(values[x]);
            }
        }

        public void WriteUInt64Array(object[] values)
        {
            WriteUInt64Array(values, 0, values.Length);
        }

        public void WriteUInt64Array(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteUInt64(values[x]);
            }
        }

        public void WriteInt64Array(object[] values)
        {
            WriteInt64Array(values, 0, values.Length);
        }

        public void WriteInt64Array(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteInt64(values[x]);
            }
        }

        public void WriteSingleArray(object[] values)
        {
            WriteSingleArray(values, 0, values.Length);
        }

        public void WriteSingleArray(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteSingle(values[x]);
            }
        }

        public void WriteDoubleArray(object[] values)
        {
            WriteDoubleArray(values, 0, values.Length);
        }

        public void WriteDoubleArray(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                WriteDouble(values[x]);
            }
        }

        public void WriteAsciiArray(object[] values, int codepage)
        {
            WriteAsciiArray(values, 0, values.Length, codepage);
        }

        public void WriteAsciiArray(object[] values, int index, int count, int codepage)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                // Use empty string if the value is null
                WriteAscii(values[x]?.ToString() ?? string.Empty, codepage);
            }
        }

        public void WriteAsciiArray(object[] values)
        {
            WriteAsciiArray(values, 0, values.Length, 1252);
        }
        public void WriteAsciiArray(object[] values, int index, int count)
        {
            WriteAsciiArray(values, index, count, 1252);
        }
        public void WriteUnicodeArray(object[] values)
        {
            WriteUnicodeArray(values, 0, values.Length);
        }
        public void WriteUnicodeArray(object[] values, int index, int count)
        {
            if (Locked)
                throw new Exception("Cannot Write to a locked Packet.");

            for (int x = index; x < index + count; ++x)
            {
                // Use empty string if the value is null
                WriteUnicode(values[x]?.ToString() ?? string.Empty);
            }
        }

        public float ReadFloat()
        {
            if (!Locked)
            {
                throw new Exception("Cannot Read from an unlocked Packet.");
            }

            return m_reader.ReadSingle();
        }

        public void WriteFloat(float value)
        {
            if (Locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }

            m_writer.Write(value);
        }

    }
}