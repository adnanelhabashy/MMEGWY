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
    /// Represents an index price message in the MME system.
    /// </summary>
    public class MmeIndexPrice : IByteMessage
    {
        public const short MESSAGE_GROUP = 13;
        public const short MESSAGE_ID = 7;
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
        /// Gets or sets the actor identifier.
        /// </summary>
        public int ActorId { get; set; }
        /// <summary>
        /// Gets or sets the order book identifier.
        /// </summary>
        public int OrderBookId { get; set; }
        /// <summary>
        /// Gets or sets the index price.
        /// </summary>
        public long Price { get; set; }
        /// <summary>
        /// Gets or sets the timestamp of the index price.
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the change percentage, represented with two decimals (e.g., 500 for 5%).
        /// </summary>
        public int ChangePercentage { get; set; } // Two decimals, e.g., 500 for 5%
        /// <summary>
        /// Writes the index price message to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutLong(ClientSequenceNumber);
            buffer.PutShort(MessageGroup);
            buffer.PutShort(MessageId);
            buffer.Put(PartitionId);
            buffer.PutInt(ActorId);
            buffer.PutInt(OrderBookId);
            buffer.PutLong(Price);
            buffer.PutLong(Timestamp);
            buffer.PutInt(ChangePercentage);

            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the index price message from the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;

            ClientSequenceNumber = buffer.GetLong();
            buffer.GetShort(); // MessageGroup
            buffer.GetShort(); // MessageId
            PartitionId = buffer.Get();
            ActorId = buffer.GetInt();
            OrderBookId = buffer.GetInt();
            Price = buffer.GetLong();
            Timestamp = buffer.GetLong();
            ChangePercentage = buffer.GetInt();

            return buffer.Position - position;
        }
    }
}
