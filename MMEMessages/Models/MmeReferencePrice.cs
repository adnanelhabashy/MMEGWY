using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
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

        public long ClientSequenceNumber
        {
            get => _clientSequenceNumber;
            set => _clientSequenceNumber = value;
        }

        public short MessageGroup
        {
            get => _messageGroup;
            set => _messageGroup = value;
        }

        public short MessageId
        {
            get => _messageId;
            set => _messageId = value;
        }

        public byte PartitionId
        {
            get => _partitionId;
            set => _partitionId = value;
        }

        public int ActorId
        {
            get => _actorId;
            set => _actorId = value;
        }

        public int OrderBookId
        {
            get => _orderBookId;
            set => _orderBookId = value;
        }

        public byte ReferencePriceSource
        {
            get => _referencePriceSource;
            set => _referencePriceSource = value;
        }

        public long ReferencePrice
        {
            get => _referencePrice;
            set => _referencePrice = value;
        }

        public long Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }

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
