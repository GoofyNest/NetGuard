namespace SilkroadSecurityAPI
{
    public class TransferBuffer
    {
        byte[] m_buffer;
        int m_offset;
        int m_size;

        public byte[] Buffer
        {
            get { return m_buffer; }
            set { m_buffer = value; }
        }

        public int Offset
        {
            get { return m_offset; }
            set { m_offset = value; }
        }

        public int Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        public TransferBuffer(TransferBuffer rhs)
        {
            m_buffer = new byte[rhs.m_buffer.Length];
            System.Buffer.BlockCopy(rhs.m_buffer, 0, m_buffer, 0, m_buffer.Length);
            m_offset = rhs.m_offset;
            m_size = rhs.m_size;
        }

        public TransferBuffer()
        {
            m_buffer = null!;
            m_offset = 0;
            m_size = 0;
        }

        public TransferBuffer(int length, int offset, int size)
        {
            m_buffer = new byte[length];
            m_offset = offset;
            m_size = size;
        }

        public TransferBuffer(int length)
        {
            m_buffer = new byte[length];
            m_offset = 0;
            m_size = 0;
        }

        public TransferBuffer(byte[] buffer, int offset, int size, bool assign)
        {
            if (assign)
            {
                m_buffer = buffer;
            }
            else
            {
                m_buffer = new byte[buffer.Length];
                System.Buffer.BlockCopy(buffer, 0, m_buffer, 0, buffer.Length);
            }
            m_offset = offset;
            m_size = size;
        }
    }
}