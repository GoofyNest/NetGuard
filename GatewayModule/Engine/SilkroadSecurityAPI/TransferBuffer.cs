using System;

namespace SilkroadSecurityAPI
{
    public class TransferBuffer
    {
        private byte[] _buffer;
        private int _offset;
        private int _size;
        private readonly object _lock = new object();

        public byte[] Buffer
        {
            get
            {
                lock (_lock)
                {
                    return _buffer;
                }
            }
            set
            {
                lock (_lock)
                {
                    _buffer = value;
                }
            }
        }

        public int Offset
        {
            get
            {
                lock (_lock)
                {
                    return _offset;
                }
            }
            set
            {
                lock (_lock)
                {
                    _offset = value;
                }
            }
        }

        public int Size
        {
            get
            {
                lock (_lock)
                {
                    return _size;
                }
            }
            set
            {
                lock (_lock)
                {
                    _size = value;
                }
            }
        }

        public TransferBuffer(TransferBuffer rhs)
        {
            if (rhs == null) throw new ArgumentNullException(nameof(rhs));

            lock (rhs._lock)
            {
                _buffer = new byte[rhs._buffer.Length];
                System.Buffer.BlockCopy(rhs._buffer, 0, _buffer, 0, rhs._buffer.Length);
                _offset = rhs._offset;
                _size = rhs._size;
            }
        }

        public TransferBuffer()
        {
            _buffer = Array.Empty<byte>();
            _offset = 0;
            _size = 0;
        }

        public TransferBuffer(int length, int offset, int size)
        {
            if (length < 0 || offset < 0 || size < 0 || offset + size > length)
                throw new ArgumentOutOfRangeException("Invalid length, offset, or size values.");

            _buffer = new byte[length];
            _offset = offset;
            _size = size;
        }

        public TransferBuffer(int length) : this(length, 0, 0) { }

        public TransferBuffer(byte[] buffer, int offset, int size, bool assign)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || size < 0 || offset + size > buffer.Length)
                throw new ArgumentOutOfRangeException("Invalid offset or size values.");

            if (assign)
            {
                _buffer = buffer;
            }
            else
            {
                _buffer = new byte[buffer.Length];
                System.Buffer.BlockCopy(buffer, 0, _buffer, 0, buffer.Length);
            }

            _offset = offset;
            _size = size;
        }
    }
}