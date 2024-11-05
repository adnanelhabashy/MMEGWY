using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    public class MmeCommit : IByteMessage
    {
        public const short COMMIT_MESSAGE_GROUP = 26;

        private short _messageGroup;
        private short _messageId;
        private byte _partitionId;
        private int _statusCode;
        private long _clientSequenceNumber;

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

        public int StatusCode
        {
            get => _statusCode;
            set => _statusCode = value;
        }

        public long ClientSequenceNumber
        {
            get => _clientSequenceNumber;
            set => _clientSequenceNumber = value;
        }

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
