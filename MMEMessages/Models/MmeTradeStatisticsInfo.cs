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
    /// Represents trade statistics information in the MME system.
    /// </summary>
    public class MmeTradeStatisticsInfo : IByteMessage
    {
        public const short MESSAGE_GROUP = 13;
        public const short MESSAGE_ID = 6;
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
        /// Gets or sets the opening price.
        /// </summary>
        public long OpenPrice { get; set; }
        /// <summary>
        /// Gets or sets the highest price.
        /// </summary>
        public long HighPrice { get; set; }
        /// <summary>
        /// Gets or sets the lowest price.
        /// </summary>
        public long LowPrice { get; set; }
        /// <summary>
        /// Gets or sets the last traded price.
        /// </summary>
        public long LastPrice { get; set; }
        /// <summary>
        /// Gets or sets the last auction price.
        /// </summary>
        public long LastAuctionPrice { get; set; }
        /// <summary>
        /// Gets or sets the last traded quantity.
        /// </summary>
        public long LastQuantity { get; set; }
        /// <summary>
        /// Gets or sets the total daily quantity traded.
        /// </summary>
        public long DailyQuantity { get; set; }
        /// <summary>
        /// Gets or sets the daily trade reported quantity.
        /// </summary>
        public long DailyTradeReportedQuantity { get; set; }
        /// <summary>
        /// Gets or sets the total daily value traded.
        /// </summary>
        public long DailyValue { get; set; }
        /// <summary>
        /// Gets or sets the timestamp of the statistics.
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// Writes the trade statistics information to the specified buffer.
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
        /// <summary>
        /// Reads the trade statistics information from the specified buffer.
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
