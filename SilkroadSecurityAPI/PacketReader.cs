using System.IO;

namespace SilkroadSecurityAPI
{
    internal class PacketReader : System.IO.BinaryReader
    {
        private readonly byte[] Input;

        public PacketReader(byte[] input)
            : base(new MemoryStream(input, false))
        {
            Input = input;
        }

        public PacketReader(byte[] input, int index, int count)
            : base(new MemoryStream(input, index, count, false))
        {
            Input = input;
        }

        public byte[] GetRawInput()
        {
            return Input;
        }
    }
}