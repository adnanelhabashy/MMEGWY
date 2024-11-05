using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    public class MmeSetExchangeRate : IByteMessage
    {
        public const short MESSAGE_GROUP = 12;
        public const short MESSAGE_ID = 31;

        public long ClientSequenceNumber { get; set; }
        public short MessageGroup => MESSAGE_GROUP;
        public short MessageId => MESSAGE_ID;
        public byte PartitionId { get; set; }

        public int ActorId { get; set; }
        public string CurrencyPair { get; set; } // String
        public long ExchangeRate { get; set; }

        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutLong(ClientSequenceNumber);
            buffer.PutShort(MessageGroup);
            buffer.PutShort(MessageId);
            buffer.Put(PartitionId);
            buffer.PutInt(ActorId);

            // Write CurrencyPair as a string
            byte[] currencyPairBytes = Encoding.ASCII.GetBytes(CurrencyPair);
            Unsigned.PutUnsignedShort(buffer, currencyPairBytes.Length);
            buffer.Put(currencyPairBytes);

            buffer.PutLong(ExchangeRate);

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

            // Read CurrencyPair
            int currencyPairLength = Unsigned.GetUnsignedShort(buffer);
            byte[] currencyPairBytes = new byte[currencyPairLength];
            buffer.Get(currencyPairBytes);
            CurrencyPair = Encoding.ASCII.GetString(currencyPairBytes);

            ExchangeRate = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
