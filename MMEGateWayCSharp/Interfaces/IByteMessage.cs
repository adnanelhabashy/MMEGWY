using DotNetty.Buffers;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Interfaces
{
    public interface IByteMessage
    {
        int Write(ByteBuffer buffer);
        int Read(ByteBuffer buffer);
    }
}
