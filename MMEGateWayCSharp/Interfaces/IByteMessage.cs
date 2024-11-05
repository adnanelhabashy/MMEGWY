using DotNetty.Buffers;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Interfaces
{
    /// <summary>
    /// Represents a message that can be read from and written to a byte buffer.
    /// </summary>
    public interface IByteMessage
    {
        /// <summary>
        /// Writes the message to the specified byte buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        int Write(ByteBuffer buffer);

        /// <summary>
        /// Reads the message from the specified byte buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
        int Read(ByteBuffer buffer);
    }
}
