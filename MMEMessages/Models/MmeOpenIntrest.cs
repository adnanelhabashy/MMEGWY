using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    public class MmeOpenInterest : IByteMessage
    {
        public const short MESSAGE_GROUP = 13;
        public const short MESSAGE_ID = 8;

        public long ClientSequenceNumber { get; set; }
        public short MessageGroup => MESSAGE_GROUP;
        public short MessageId => MESSAGE_ID;
        public byte PartitionId { get; set; }

        public int ActorId { get; set; }
        public int OrderBookId { get; set; }
        public long OpenInterestValue { get; set; }
        public long Timestamp { get; set; }

        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutLong(ClientSequenceNumber);
            buffer.PutShort(MessageGroup);
            buffer.PutShort(MessageId);
            buffer.Put(PartitionId);
            buffer.PutInt(ActorId);
            buffer.PutInt(OrderBookId);
            buffer.PutLong(OpenInterestValue);
            buffer.PutLong(Timestamp);

            return buffer.Position - position;
        }

        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;

            ClientSequenceNumber = buffer.GetLong();
            buffer.GetShort(); // MessageGroup
            buffer.GetShort(); // MessageId
            PartitionId = buffer.Get();
            ActorId = buffer.GetInt();
            OrderBookId = buffer.GetInt();
            OpenInterestValue = buffer.GetLong();
            Timestamp = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
