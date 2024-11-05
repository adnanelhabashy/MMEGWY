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
    /// Represents a SOUP protocol header.
    /// </summary>
    public class SoupHeader : IByteMessage
    {
        /// <summary>
        /// Gets or sets the total length of the message (including the type byte).
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public byte Type { get; set; }
        /// <summary>
        /// Gets the length of the payload (excluding the type byte).
        /// </summary>
        public int PayloadLength => Length - 1;
        /// <summary>
        /// Writes the header to the specified ByteBuffer.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;
            Unsigned.PutUnsignedShort(buffer, Length);
            Unsigned.PutUnsignedByte(buffer, Type);
            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the header from the specified ByteBuffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;
            Length = Unsigned.GetUnsignedShort(buffer);
            Type = (byte)Unsigned.GetUnsignedByte(buffer);
            return buffer.Position - position;
        }
    }
}
