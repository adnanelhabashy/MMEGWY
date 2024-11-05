using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    public class MmeEnterBusinessDate : IByteMessage
    {
        public const short MESSAGE_GROUP = 5;
        public const short MESSAGE_ID = 18;

        public long ClientSequenceNumber { get; set; }
        public short MessageGroup => MESSAGE_GROUP;
        public short MessageId => MESSAGE_ID;
        public byte PartitionId { get; set; }

        public int ActorId { get; set; }
        public int MarketId { get; set; }
        public long BusinessDate { get; set; } // Epoch nanoseconds UTC

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
