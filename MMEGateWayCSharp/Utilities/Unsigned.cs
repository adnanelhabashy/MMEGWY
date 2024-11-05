using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Utilities
{
    /// <summary>
    /// Provides methods for handling unsigned data types in byte buffers.
    /// </summary>
    public static class Unsigned
    {
        /// <summary>
        /// Reads an unsigned byte from the ByteBuffer.
        /// </summary>
        /// <param name="bb">The ByteBuffer to read from.</param>
        /// <returns>The unsigned byte value.</returns>
        public static ushort GetUnsignedByte(ByteBuffer bb)
        {
            return (ushort)(bb.Get() & 0xff);
        }
        /// <summary>
        /// Writes an unsigned byte to the ByteBuffer.
        /// </summary>
        /// <param name="bb">The ByteBuffer to write to.</param>
        /// <param name="value">The unsigned byte value to write.</param>
        public static void PutUnsignedByte(ByteBuffer bb, int value)
        {
            bb.Put((byte)(value & 0xff));
        }
        /// <summary>
        /// Reads an unsigned short from the ByteBuffer.
        /// </summary>
        /// <param name="bb">The ByteBuffer to read from.</param>
        /// <returns>The unsigned short value.</returns>
        public static ushort GetUnsignedShort(ByteBuffer bb)
        {
            return (ushort)(bb.GetShort() & 0xffff);
        }
        /// <summary>
        /// Writes an unsigned short to the ByteBuffer.
        /// </summary>
        /// <param name="bb">The ByteBuffer to write to.</param>
        /// <param name="value">The unsigned short value to write.</param>
        public static void PutUnsignedShort(ByteBuffer bb, int value)
        {
            bb.PutShort((short)(value & 0xffff));
        }
        /// <summary>
        /// Reads an unsigned int from the ByteBuffer.
        /// </summary>
        /// <param name="bb">The ByteBuffer to read from.</param>
        /// <returns>The unsigned int value.</returns>
        public static uint GetUnsignedInt(ByteBuffer bb)
        {
            return (uint)(bb.GetInt() & 0xffffffff);
        }
        /// <summary>
        /// Writes an unsigned int to the ByteBuffer.
        /// </summary>
        /// <param name="bb">The ByteBuffer to write to.</param>
        /// <param name="value">The unsigned int value to write.</param>
        public static void PutUnsignedInt(ByteBuffer bb, long value)
        {
            bb.PutInt((int)(value & 0xffffffff));
        }
    }
}
