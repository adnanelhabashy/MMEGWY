using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Utilities
{
    public class ByteBuffer
    {
        private byte[] _buffer;
        private int _position;
        private int _limit;
        private int _capacity;
        private ByteOrder _order = ByteOrder.BigEndian;

        private ByteBuffer(int capacity)
        {
            _buffer = new byte[capacity];
            _capacity = capacity;
            _limit = capacity;
            _position = 0;
        }

        public static ByteBuffer Allocate(int capacity)
        {
            return new ByteBuffer(capacity);
        }

        public int Capacity => _capacity;

        public int Position
        {
            get => _position;
            set => _position = value;
        }

        public int Limit
        {
            get => _limit;
            set => _limit = value;
        }

        public bool HasRemaining => _position < _limit;

        public int Remaining => _limit - _position;

        public void Clear()
        {
            _position = 0;
            _limit = _capacity;
        }

        public void Flip()
        {
            _limit = _position;
            _position = 0;
        }

        public void Put(byte b)
        {
            _buffer[_position++] = b;
        }

        public void Put(byte[] src)
        {
            Array.Copy(src, 0, _buffer, _position, src.Length);
            _position += src.Length;
        }

        public void Put(byte[] src, int offset, int length)
        {
            Array.Copy(src, offset, _buffer, _position, length);
            _position += length;
        }

        public byte Get()
        {
            return _buffer[_position++];
        }

        public void Get(byte[] dst)
        {
            Array.Copy(_buffer, _position, dst, 0, dst.Length);
            _position += dst.Length;
        }

        public byte[] ToArray()
        {
            var result = new byte[_limit];
            Array.Copy(_buffer, 0, result, 0, _limit);
            return result;
        }

        public ByteBuffer AsReadOnlyBuffer()
        {
            var readOnlyBuffer = new ByteBuffer(_limit)
            {
                _buffer = (byte[])_buffer.Clone(),
                _position = _position,
                _limit = _limit,
                _capacity = _capacity,
                _order = _order
            };
            return readOnlyBuffer;
        }

        public ByteOrder Order
        {
            get => _order;
            set => _order = value;
        }

        public void PutShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            if ((_order == ByteOrder.BigEndian) != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            Put(bytes);
        }

        public short GetShort()
        {
            var bytes = new byte[2];
            Get(bytes);
            if ((_order == ByteOrder.BigEndian) != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt16(bytes, 0);
        }

        public void PutLong(long value)
        {
            EnsureCapacity(8);
            var bytes = BitConverter.GetBytes(value);
            AdjustByteOrder(bytes);
            Put(bytes);
        }

        public long GetLong()
        {
            var bytes = new byte[8];
            Get(bytes);
            AdjustByteOrder(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public void PutInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if ((_order == ByteOrder.BigEndian) != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            Put(bytes);
        }

        public int GetInt()
        {
            var bytes = new byte[4];
            Get(bytes);
            if ((_order == ByteOrder.BigEndian) != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }
        private void EnsureCapacity(int additionalCapacity)
        {
            if (_position + additionalCapacity > _capacity)
            {
                throw new InvalidOperationException("Buffer overflow");
            }
        }

        private void AdjustByteOrder(byte[] bytes)
        {
            if ((_order == ByteOrder.BigEndian) != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
        }

        public int RemainingCapacity => _capacity - _position;
    }
}
