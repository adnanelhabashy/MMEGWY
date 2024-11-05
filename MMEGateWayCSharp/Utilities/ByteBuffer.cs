using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Utilities
{
    /// <summary>
    /// Represents a resizable byte buffer similar to Java's ByteBuffer.
    /// </summary>
    public class ByteBuffer
    {
        private byte[] _buffer;
        private int _position;
        private int _limit;
        private int _capacity;
        private ByteOrder _order = ByteOrder.BigEndian;
        /// <summary>
        /// Initializes a new instance of the ByteBuffer class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the buffer.</param>
        private ByteBuffer(int capacity)
        {
            _buffer = new byte[capacity];
            _capacity = capacity;
            _limit = capacity;
            _position = 0;
        }
        /// <summary>
        /// Allocates a new ByteBuffer with the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity to allocate.</param>
        /// <returns>A new ByteBuffer instance.</returns>
        public static ByteBuffer Allocate(int capacity)
        {
            return new ByteBuffer(capacity);
        }
        /// <summary>
        /// Gets the capacity of the buffer.
        /// </summary>
        public int Capacity => _capacity;
        /// <summary>
        /// Gets or sets the current position in the buffer.
        /// </summary>
        public int Position
        {
            get => _position;
            set => _position = value;
        }
        /// <summary>
        /// Gets or sets the limit of the buffer.
        /// </summary>
        public int Limit
        {
            get => _limit;
            set => _limit = value;
        }
        /// <summary>
        /// Gets a value indicating whether there are remaining bytes between the current position and the limit.
        /// </summary>
        public bool HasRemaining => _position < _limit;
        /// <summary>
        /// Gets the number of remaining bytes between the current position and the limit.
        /// </summary>
        public int Remaining => _limit - _position;
        /// <summary>
        /// Clears the buffer, resetting position and limit.
        /// </summary>
        public void Clear()
        {
            _position = 0;
            _limit = _capacity;
        }
        /// <summary>
        /// Flips the buffer, setting the limit to the current position and resetting the position to zero.
        /// </summary>
        public void Flip()
        {
            _limit = _position;
            _position = 0;
        }
        /// <summary>
        /// Puts a byte into the buffer at the current position.
        /// </summary>
        /// <param name="b">The byte to put.</param>
        public void Put(byte b)
        {
            _buffer[_position++] = b;
        }
        /// <summary>
        /// Puts an array of bytes into the buffer starting at the current position.
        /// </summary>
        /// <param name="src">The source array.</param>
        public void Put(byte[] src)
        {
            Array.Copy(src, 0, _buffer, _position, src.Length);
            _position += src.Length;
        }
        /// <summary>
        /// Puts a range of bytes from an array into the buffer starting at the current position.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="offset">The zero-based byte offset in src.</param>
        /// <param name="length">The number of bytes to copy.</param>
        public void Put(byte[] src, int offset, int length)
        {
            Array.Copy(src, offset, _buffer, _position, length);
            _position += length;
        }
        /// <summary>
        /// Gets a byte from the buffer at the current position.
        /// </summary>
        /// <returns>The byte at the current position.</returns>
        public byte Get()
        {
            return _buffer[_position++];
        }
        /// <summary>
        /// Reads bytes from the buffer into the specified array.
        /// </summary>
        /// <param name="dst">The destination array.</param>
        public void Get(byte[] dst)
        {
            Array.Copy(_buffer, _position, dst, 0, dst.Length);
            _position += dst.Length;
        }
        /// <summary>
        /// Converts the buffer into a byte array.
        /// </summary>
        /// <returns>A byte array containing the buffer's data.</returns>
        public byte[] ToArray()
        {
            var result = new byte[_limit];
            Array.Copy(_buffer, 0, result, 0, _limit);
            return result;
        }
        /// <summary>
        /// Creates a read-only copy of the buffer.
        /// </summary>
        /// <returns>A read-only ByteBuffer.</returns>
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
        /// <summary>
        /// Gets or sets the byte order of the buffer.
        /// </summary>
        public ByteOrder Order
        {
            get => _order;
            set => _order = value;
        }
        /// <summary>
        /// Puts a short integer into the buffer at the current position.
        /// </summary>
        /// <param name="value">The short integer to put.</param>
        public void PutShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            if ((_order == ByteOrder.BigEndian) != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            Put(bytes);
        }
        /// <summary>
        /// Gets a short integer from the buffer at the current position.
        /// </summary>
        /// <returns>The short integer.</returns>
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
        /// <summary>
        /// Puts a long integer into the buffer at the current position.
        /// </summary>
        /// <param name="value">The long integer to put.</param>
        public void PutLong(long value)
        {
            EnsureCapacity(8);
            var bytes = BitConverter.GetBytes(value);
            AdjustByteOrder(bytes);
            Put(bytes);
        }
        /// <summary>
        /// Gets a long integer from the buffer at the current position.
        /// </summary>
        /// <returns>The long integer.</returns>
        public long GetLong()
        {
            var bytes = new byte[8];
            Get(bytes);
            AdjustByteOrder(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
        /// <summary>
        /// Puts an integer into the buffer at the current position.
        /// </summary>
        /// <param name="value">The integer to put.</param>
        public void PutInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if ((_order == ByteOrder.BigEndian) != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            Put(bytes);
        }
        /// <summary>
        /// Gets an integer from the buffer at the current position.
        /// </summary>
        /// <returns>The integer.</returns>
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
        /// <summary>
        /// Ensures that the buffer has enough capacity for additional data.
        /// </summary>
        /// <param name="additionalCapacity">The additional capacity needed.</param>
        private void EnsureCapacity(int additionalCapacity)
        {
            if (_position + additionalCapacity > _capacity)
            {
                throw new InvalidOperationException("Buffer overflow");
            }
        }
        /// <summary>
        /// Adjusts the byte order of the byte array to match the buffer's byte order.
        /// </summary>
        /// <param name="bytes">The byte array to adjust.</param>
        private void AdjustByteOrder(byte[] bytes)
        {
            if ((_order == ByteOrder.BigEndian) != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
        }
        /// <summary>
        /// Gets the remaining capacity of the buffer.
        /// </summary>
        public int RemainingCapacity => _capacity - _position;
    }
}
