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
    /// Represents a message to cancel a deal in the MME system.
    /// </summary>
    public class MmeCancelDeal : IByteMessage
    {
        public const short MESSAGE_GROUP = 14;
        public const short MESSAGE_ID = 3;
        /// <summary>
        /// Gets or sets the client sequence number.
        /// </summary>
        public long ClientSequenceNumber { get; set; }
        /// <summary>
        /// Gets the message group identifier.
        /// </summary>
        public short MessageGroup => MESSAGE_GROUP;
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        public short MessageId => MESSAGE_ID;
        /// <summary>
        /// Gets or sets the partition identifier.
        /// </summary>
        public byte PartitionId { get; set; }
        /// <summary>
        /// Gets or sets the order book identifier.
        /// </summary>
        public int OrderBookId { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the deal identifier to be cancelled.
        /// </summary>
        public long DealId { get; set; }
        /// <summary>
        /// Writes the message data to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the data to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutLong(ClientSequenceNumber);
            buffer.PutShort(MessageGroup);
            buffer.PutShort(MessageId);
            buffer.Put(PartitionId);
            buffer.PutInt(OrderBookId);
            buffer.PutInt(UserId);
            buffer.PutLong(DealId);

            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the message data from the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read the data from.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;

            ClientSequenceNumber = buffer.GetLong();
            buffer.GetShort(); // MessageGroup
            buffer.GetShort(); // MessageId
            PartitionId = buffer.Get();
            OrderBookId = buffer.GetInt();
            UserId = buffer.GetInt();
            DealId = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
