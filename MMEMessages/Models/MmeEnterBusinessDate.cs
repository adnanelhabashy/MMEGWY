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
    /// Represents a message to enter a business date in the MME system.
    /// </summary>
    public class MmeEnterBusinessDate : IByteMessage
    {
        public const short MESSAGE_GROUP = 5;
        public const short MESSAGE_ID = 18;
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
        /// Gets or sets the market identifier.
        /// </summary>
        public int MarketId { get; set; }
        /// <summary>
        /// Gets or sets the business date in epoch nanoseconds UTC.
        /// </summary>
        public long BusinessDate { get; set; } // Epoch nanoseconds UTC
        /// <summary>
        /// Writes the enter business date message to the specified buffer.
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
            buffer.PutInt(MarketId);
            buffer.PutLong(BusinessDate);

            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the enter business date message from the specified buffer.
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
            MarketId = buffer.GetInt();
            BusinessDate = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
