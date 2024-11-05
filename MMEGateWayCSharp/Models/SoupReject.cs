using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Models
{
    /// <summary>
    /// Represents a SOUP protocol reject message.
    /// </summary>
    public class SoupReject : IByteMessage
    {
        /// <summary>
        /// Gets or sets the reason code for rejection.
        /// </summary>
        public char Reason { get; set; }
        /// <summary>
        /// Writes the reject message to the specified ByteBuffer.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;
            // No implementation needed for write in this case
            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the reject message from the specified ByteBuffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;
            Reason = (char)buffer.Get();
            return buffer.Position - position;
        }
    }
}
