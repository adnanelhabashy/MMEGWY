using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Utilities
{
    public static class Unsigned
    {
        public static ushort GetUnsignedByte(ByteBuffer bb)
        {
            return (ushort)(bb.Get() & 0xff);
        }

        public static void PutUnsignedByte(ByteBuffer bb, int value)
        {
            bb.Put((byte)(value & 0xff));
        }

        public static ushort GetUnsignedShort(ByteBuffer bb)
        {
            return (ushort)(bb.GetShort() & 0xffff);
        }

        public static void PutUnsignedShort(ByteBuffer bb, int value)
        {
            bb.PutShort((short)(value & 0xffff));
        }

        public static uint GetUnsignedInt(ByteBuffer bb)
        {
            return (uint)(bb.GetInt() & 0xffffffff);
        }

        public static void PutUnsignedInt(ByteBuffer bb, long value)
        {
            bb.PutInt((int)(value & 0xffffffff));
        }
    }
}
