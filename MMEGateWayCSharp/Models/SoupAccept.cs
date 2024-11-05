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
    public class SoupAccept : IByteMessage
    {
        private string _session;
        private int _sequenceNumber;

        public string Session
        {
            get => _session;
            set => _session = value;
        }

        public int SequenceNumber
        {
            get => _sequenceNumber;
            set => _sequenceNumber = value;
        }

        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(_session, SoupConstants.SESSION_LENGTH)));
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(_sequenceNumber, SoupConstants.SEQUENCE_NUMBER_LENGTH)));
            return buffer.Position - position;
        }

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
