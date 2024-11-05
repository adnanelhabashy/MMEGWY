using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    public class ServerCommitResponseMessage : IByteMessage
    {
        public const short MESSAGE_GROUP = 26;
        public const short MESSAGE_ID = 1;

        public byte PartitionId { get; set; }
        public int StatusCode { get; set; } // Negative if request failed
        public long ClientSequenceNumber { get; set; }

        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutShort(MESSAGE_GROUP);
            buffer.PutShort(MESSAGE_ID);
            buffer.Put(PartitionId);
            buffer.PutInt(StatusCode);
            buffer.PutLong(ClientSequenceNumber);

            return buffer.Position - position;
        }

        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.GetShort(); // MessageGroup
            buffer.GetShort(); // MessageId
            PartitionId = buffer.Get();
            StatusCode = buffer.GetInt();
            ClientSequenceNumber = buffer.GetLong();

            return buffer.Position - position;
        }
    }
}
