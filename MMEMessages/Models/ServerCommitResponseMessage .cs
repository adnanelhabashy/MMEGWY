using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    /// <summary>
    /// Represents a server commit response message in the MME system.
    /// </summary>
    public class ServerCommitResponseMessage : IByteMessage
    {
        public const short MESSAGE_GROUP = 26;
        public const short MESSAGE_ID = 1;
        /// <summary>
        /// Gets or sets the partition identifier.
        /// </summary>
        public byte PartitionId { get; set; }
        /// <summary>
        /// Gets or sets the status code of the response. Negative if the request failed.
        /// </summary>
        public int StatusCode { get; set; } // Negative if request failed
        /// <summary>
        /// Gets or sets the client sequence number.
        /// </summary>
        public long ClientSequenceNumber { get; set; }
        /// <summary>
        /// Writes the server commit response message to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutShort(MESSAGE_GROUP);
            buffer.PutShort(MESSAGE_ID);
            buffer.Put(PartitionId);
            buffer.PutInt(StatusCode);
            buffer.PutLong(ClientSequenceNumber);

            return buffer.Position - position;
        }

        /// <summary>
        /// Reads the server commit response message from the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.GetShort(); // MessageGroup
            buffer.GetShort(); // MessageId
            PartitionId = buffer.Get();
            StatusCode = buffer.GetInt();
            ClientSequenceNumber = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
