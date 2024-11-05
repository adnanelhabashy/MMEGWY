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
    /// Represents a commit message in the MME system.
    /// </summary>
    public class MmeCommit : IByteMessage
    {
        public const short COMMIT_MESSAGE_GROUP = 26;

        private short _messageGroup;
        private short _messageId;
        private byte _partitionId;
        private int _statusCode;
        private long _clientSequenceNumber;
        /// <summary>
        /// Gets or sets the message group.
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
        /// Gets or sets the status code of the commit.
        /// </summary>
        public int StatusCode
        {
            get => _statusCode;
            set => _statusCode = value;
        }
        /// <summary>
        /// Gets or sets the client sequence number.
        /// </summary>
        public long ClientSequenceNumber
        {
            get => _clientSequenceNumber;
            set => _clientSequenceNumber = value;
        }
        /// <summary>
        /// Writes the commit message to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutShort(_messageGroup);
            buffer.PutShort(_messageId);
            buffer.Put(_partitionId);
            buffer.PutInt(_statusCode);
            buffer.PutLong(_clientSequenceNumber);

            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the commit message from the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;

            _messageGroup = buffer.GetShort();
            _messageId = buffer.GetShort();
            _partitionId = buffer.Get();
            _statusCode = buffer.GetInt();
            _clientSequenceNumber = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
