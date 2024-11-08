﻿using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Models
{
    /// <summary>
    /// Represents a message to enter a schedule override in the MME system.
    /// </summary>
    public class MmeEnterScheduleOverride : IByteMessage
    {
        public const short MESSAGE_GROUP = 5;
        public const short MESSAGE_ID = 1;
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
        /// Gets or sets the override number.
        /// </summary>
        public long OverrideNumber { get; set; }
        /// <summary>
        /// Gets or sets the actor identifier.
        /// </summary>
        public int ActorId { get; set; }
        /// <summary>
        /// Gets or sets the list of session items.
        /// </summary>
        public List<SessionItem> SessionItems { get; set; } = new List<SessionItem>();
        /// <summary>
        /// Gets or sets the list of entity items.
        /// </summary>
        public List<EntityItem> EntityItems { get; set; } = new List<EntityItem>();
        /// <summary>
        /// Writes the message data to the specified buffer.
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
        /// <summary>
        /// Reads the message data from the specified buffer.
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

    /// <summary>
    /// Represents a session item within a schedule override.
    /// </summary>
    public class SessionItem
    {
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the order in which the session occurs.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Gets or sets the scheduled start time in epoch nanoseconds UTC.
        /// </summary>
        public long StartTime { get; set; } // Epoch nanoseconds UTC
        /// <summary>
        /// Gets or sets the actual start time. Always 0.
        /// </summary>
        public long ActualStartTime { get; set; } // Always 0
        /// <summary>
        /// Gets or sets the start offset. Always 0.
        /// </summary>
        public long StartOffset { get; set; } // Always 0
        /// <summary>
        /// Gets or sets the random interval.
        /// </summary>
        public int RandomInterval { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the default halt is enabled.
        /// </summary>
        public bool DefaultHalt { get; set; }
        /// <summary>
        /// Gets or sets the order action.
        /// </summary>
        public byte OrderAction { get; set; }
    }
    /// <summary>
    /// Represents an entity item within a schedule override.
    /// </summary>
    public class EntityItem
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the level of the entity (e.g., 1 for Market, 2 for Market Segment).
        /// </summary>
        public int Level { get; set; } // 1: Market, 2: Market Segment, etc.
    }
}
