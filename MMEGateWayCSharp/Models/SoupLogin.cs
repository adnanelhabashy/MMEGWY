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
    public class SoupLogin : IByteMessage
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Session { get; set; }
        public long SequenceNumber { get; set; }

        public SoupLogin()
        {
        }

        public SoupLogin(string userName, string password, string session, long sequenceNumber)
        {
            UserName = userName;
            Password = password;
            Session = session;
            SequenceNumber = sequenceNumber;
        }

        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(UserName, SoupConstants.USER_NAME_LENGTH)));
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(Password, SoupConstants.PASSWORD_LENGTH)));
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(Session, SoupConstants.SESSION_LENGTH)));
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(SequenceNumber, SoupConstants.SEQUENCE_NUMBER_LENGTH)));
            return buffer.Position - position;
        }

        public int Read(ByteBuffer buffer)
        {
            int position = buffer.Position;
            byte[] aUserName = new byte[SoupConstants.USER_NAME_LENGTH];
            buffer.Get(aUserName);
            UserName = Encoding.ASCII.GetString(aUserName);
            byte[] aPassword = new byte[SoupConstants.PASSWORD_LENGTH];
            buffer.Get(aPassword);
            Password = Encoding.ASCII.GetString(aPassword);
            byte[] aSession = new byte[SoupConstants.SESSION_LENGTH];
            buffer.Get(aSession);
            Session = Encoding.ASCII.GetString(aSession);
            byte[] aSequenceNumber = new byte[SoupConstants.SEQUENCE_NUMBER_LENGTH];
            buffer.Get(aSequenceNumber);
            SequenceNumber = long.Parse(Encoding.ASCII.GetString(aSequenceNumber).Trim());
            return buffer.Position - position;
        }
    }
}
