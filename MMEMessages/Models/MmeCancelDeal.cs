using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    public class MmeCancelDeal : IByteMessage
    {
        public const short MESSAGE_GROUP = 14;
        public const short MESSAGE_ID = 3;

        public long ClientSequenceNumber { get; set; }
        public short MessageGroup => MESSAGE_GROUP;
        public short MessageId => MESSAGE_ID;
        public byte PartitionId { get; set; }

        public int OrderBookId { get; set; }
        public int UserId { get; set; }
        public long DealId { get; set; }

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
