using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Models
{
    public class SoupHeader : IByteMessage
    {
        public int Length { get; set; }
        public byte Type { get; set; }

        public int PayloadLength => Length - 1;

        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;
            Unsigned.PutUnsignedShort(buffer, Length);
            Unsigned.PutUnsignedByte(buffer, Type);
            return buffer.Position - position;
        }

        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;
            Length = Unsigned.GetUnsignedShort(buffer);
            Type = (byte)Unsigned.GetUnsignedByte(buffer);
            return buffer.Position - position;
        }
    }
}
