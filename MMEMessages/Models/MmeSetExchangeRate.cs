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
    /// Represents a message to set the exchange rate in the MME system.
    /// </summary>
    public class MmeSetExchangeRate : IByteMessage
    {
        public const short MESSAGE_GROUP = 12;
        public const short MESSAGE_ID = 31;
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
        /// Gets or sets the currency pair as a string (e.g., "USD/EUR").
        /// </summary>
        public string CurrencyPair { get; set; } // String
        /// <summary>
        /// Gets or sets the exchange rate.
        /// </summary>
        public long ExchangeRate { get; set; }

        /// <summary>
        /// Writes the set exchange rate message to the specified buffer.
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

            // Write CurrencyPair as a string
            byte[] currencyPairBytes = Encoding.ASCII.GetBytes(CurrencyPair);
            Unsigned.PutUnsignedShort(buffer, currencyPairBytes.Length);
            buffer.Put(currencyPairBytes);

            buffer.PutLong(ExchangeRate);

            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the set exchange rate message from the specified buffer.
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
