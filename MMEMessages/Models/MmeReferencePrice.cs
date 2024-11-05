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
    /// Represents a reference price message in the MME system.
    /// </summary>
    public class MmeReferencePrice : IByteMessage
    {
        private long _clientSequenceNumber;
        private short _messageGroup;
        private short _messageId;
        private byte _partitionId;
        private int _actorId;
        private int _orderBookId;
        private byte _referencePriceSource;
        private long _referencePrice;
        private long _timestamp;
        /// <summary>
        /// Gets or sets the client sequence number.
        /// </summary>
        public long ClientSequenceNumber
        {
            get => _clientSequenceNumber;
            set => _clientSequenceNumber = value;
        }
        /// <summary>
        /// Gets or sets the message group identifier.
        /// </summary>
        public short MessageGroup
        {
            get => _messageGroup;
            set => _messageGroup = value;
        }
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        public short MessageId
        {
            get => _messageId;
            set => _messageId = value;
        }
        /// <summary>
        /// Gets or sets the partition identifier.
        /// </summary>
        public byte PartitionId
        {
            get => _partitionId;
            set => _partitionId = value;
        }
        /// <summary>
        /// Gets or sets the actor identifier.
        /// </summary>
        public int ActorId
        {
            get => _actorId;
            set => _actorId = value;
        }

        /// <summary>
        /// Gets or sets the order book identifier.
        /// </summary>
        public int OrderBookId
        {
            get => _orderBookId;
            set => _orderBookId = value;
        }
        /// <summary>
        /// Gets or sets the reference price source.
        /// </summary>
        public byte ReferencePriceSource
        {
            get => _referencePriceSource;
            set => _referencePriceSource = value;
        }
        /// <summary>
        /// Gets or sets the reference price.
        /// </summary>
        public long ReferencePrice
        {
            get => _referencePrice;
            set => _referencePrice = value;
        }
        /// <summary>
        /// Gets or sets the timestamp of the reference price.
        /// </summary>
        public long Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }
        /// <summary>
        /// Writes the reference price message to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutLong(_clientSequenceNumber);
            buffer.PutShort(_messageGroup);
            buffer.PutShort(_messageId);
            buffer.Put(_partitionId);
            buffer.PutInt(_actorId);
            buffer.PutInt(_orderBookId);
            buffer.Put(_referencePriceSource);
            buffer.PutLong(_referencePrice);
            buffer.PutLong(_timestamp);

            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the reference price message from the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;

            _clientSequenceNumber = buffer.GetLong();
            _messageGroup = buffer.GetShort();
            _messageId = buffer.GetShort();
            _partitionId = buffer.Get();
            _actorId = buffer.GetInt();
            _orderBookId = buffer.GetInt();
            _referencePriceSource = buffer.Get();
            _referencePrice = buffer.GetLong();
            _timestamp = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
