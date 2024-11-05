using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    public class MmeEnterScheduleOverride : IByteMessage
    {
        public const short MESSAGE_GROUP = 5;
        public const short MESSAGE_ID = 1;

        public long ClientSequenceNumber { get; set; }
        public short MessageGroup => MESSAGE_GROUP;
        public short MessageId => MESSAGE_ID;
        public byte PartitionId { get; set; }

        public long OverrideNumber { get; set; }
        public int ActorId { get; set; }
        public List<SessionItem> SessionItems { get; set; } = new List<SessionItem>();
        public List<EntityItem> EntityItems { get; set; } = new List<EntityItem>();

        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;

            buffer.PutLong(ClientSequenceNumber);
            buffer.PutShort(MessageGroup);
            buffer.PutShort(MessageId);
            buffer.Put(PartitionId);
            buffer.PutLong(OverrideNumber);
            buffer.PutInt(ActorId);

            // Write SessionItems array
            Unsigned.PutUnsignedShort(buffer, SessionItems.Count);
            foreach (var item in SessionItems)
            {
                buffer.PutInt(item.Id);
                buffer.PutInt(item.Order);
                buffer.PutLong(item.StartTime);
                buffer.PutLong(item.ActualStartTime);
                buffer.PutLong(item.StartOffset);
                buffer.PutInt(item.RandomInterval);
                buffer.Put((byte)(item.DefaultHalt ? 1 : 0));
                buffer.Put(item.OrderAction);
            }

            // Write EntityItems array
            Unsigned.PutUnsignedShort(buffer, EntityItems.Count);
            foreach (var entity in EntityItems)
            {
                buffer.PutInt(entity.Id);
                buffer.PutInt(entity.Level);
            }

            return buffer.Position - position;
        }

        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;

            ClientSequenceNumber = buffer.GetLong();
            buffer.GetShort(); // MessageGroup
            buffer.GetShort(); // MessageId
            PartitionId = buffer.Get();
            OverrideNumber = buffer.GetLong();
            ActorId = buffer.GetInt();

            // Read SessionItems array
            int sessionItemCount = Unsigned.GetUnsignedShort(buffer);
            for (int i = 0; i < sessionItemCount; i++)
            {
                var item = new SessionItem
                {
                    Id = buffer.GetInt(),
                    Order = buffer.GetInt(),
                    StartTime = buffer.GetLong(),
                    ActualStartTime = buffer.GetLong(),
                    StartOffset = buffer.GetLong(),
                    RandomInterval = buffer.GetInt(),
                    DefaultHalt = buffer.Get() == 1,
                    OrderAction = buffer.Get()
                };
                SessionItems.Add(item);
            }

            // Read EntityItems array
            int entityItemCount = Unsigned.GetUnsignedShort(buffer);
            for (int i = 0; i < entityItemCount; i++)
            {
                var entity = new EntityItem
                {
                    Id = buffer.GetInt(),
                    Level = buffer.GetInt()
                };
                EntityItems.Add(entity);
            }

            return buffer.Position - position;
        }
    }

    public class SessionItem
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public long StartTime { get; set; } // Epoch nanoseconds UTC
        public long ActualStartTime { get; set; } // Always 0
        public long StartOffset { get; set; } // Always 0
        public int RandomInterval { get; set; }
        public bool DefaultHalt { get; set; }
        public byte OrderAction { get; set; }
    }

    public class EntityItem
    {
        public int Id { get; set; }
        public int Level { get; set; } // 1: Market, 2: Market Segment, etc.
    }
}
