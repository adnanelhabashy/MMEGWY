using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    public class MmeTradeStatisticsInfo : IByteMessage
    {
        public const short MESSAGE_GROUP = 13;
        public const short MESSAGE_ID = 6;

        public long ClientSequenceNumber { get; set; }
        public short MessageGroup => MESSAGE_GROUP;
        public short MessageId => MESSAGE_ID;
        public byte PartitionId { get; set; }

        public int ActorId { get; set; }
        public int OrderBookId { get; set; }
        public long OpenPrice { get; set; }
        public long HighPrice { get; set; }
        public long LowPrice { get; set; }
        public long LastPrice { get; set; }
        public long LastAuctionPrice { get; set; }
        public long LastQuantity { get; set; }
        public long DailyQuantity { get; set; }
        public long DailyTradeReportedQuantity { get; set; }
        public long DailyValue { get; set; }
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
            buffer.PutLong(OpenPrice);
            buffer.PutLong(HighPrice);
            buffer.PutLong(LowPrice);
            buffer.PutLong(LastPrice);
            buffer.PutLong(LastAuctionPrice);
            buffer.PutLong(LastQuantity);
            buffer.PutLong(DailyQuantity);
            buffer.PutLong(DailyTradeReportedQuantity);
            buffer.PutLong(DailyValue);
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
            OpenPrice = buffer.GetLong();
            HighPrice = buffer.GetLong();
            LowPrice = buffer.GetLong();
            LastPrice = buffer.GetLong();
            LastAuctionPrice = buffer.GetLong();
            LastQuantity = buffer.GetLong();
            DailyQuantity = buffer.GetLong();
            DailyTradeReportedQuantity = buffer.GetLong();
            DailyValue = buffer.GetLong();
            Timestamp = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
