using MMEGateWayCSharp.Core;
using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Models
{
    /// <summary>
    /// Represents a SOUP protocol accept message.
    /// </summary>
    public class SoupAccept : IByteMessage
    {
        private string _session;
        private int _sequenceNumber;
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        public string Session
        {
            get => _session;
            set => _session = value;
        }
        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        public int SequenceNumber
        {
            get => _sequenceNumber;
            set => _sequenceNumber = value;
        }
        /// <summary>
        /// Writes the accept message to the specified ByteBuffer.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(_session, SoupConstants.SESSION_LENGTH)));
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(_sequenceNumber, SoupConstants.SEQUENCE_NUMBER_LENGTH)));
            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the accept message from the specified ByteBuffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;
            byte[] aSession = new byte[SoupConstants.SESSION_LENGTH];
            buffer.Get(aSession);
            _session = Encoding.ASCII.GetString(aSession);
            byte[] aSequenceNumber = new byte[SoupConstants.SEQUENCE_NUMBER_LENGTH];
            buffer.Get(aSequenceNumber);
            _sequenceNumber = int.Parse(Encoding.ASCII.GetString(aSequenceNumber).Trim());
            return buffer.Position - position;
        }
    }
}
