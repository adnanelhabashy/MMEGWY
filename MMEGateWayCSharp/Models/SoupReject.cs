using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Models
{
    public class SoupReject : IByteMessage
    {
        public char Reason { get; set; }

        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;
            // No implementation needed for write in this case
            return buffer.Position - position;
        }

        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;
            Reason = (char)buffer.Get();
            return buffer.Position - position;
        }
    }
}
