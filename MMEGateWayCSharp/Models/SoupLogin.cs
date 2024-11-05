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
    /// Represents a SOUP protocol login message.
    /// </summary>
    public class SoupLogin : IByteMessage
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        public string Session { get; set; }
        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        public long SequenceNumber { get; set; }
        /// <summary>
        /// Initializes a new instance of the SoupLogin class.
        /// </summary>
        public SoupLogin()
        {
        }
        /// <summary>
        /// Initializes a new instance of the SoupLogin class with the specified parameters.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="session">The session identifier.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        public SoupLogin(string userName, string password, string session, long sequenceNumber)
        {
            UserName = userName;
            Password = password;
            Session = session;
            SequenceNumber = sequenceNumber;
        }
        /// <summary>
        /// Writes the login message to the specified ByteBuffer.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(ByteBuffer buffer)
        {
            int position = buffer.Position;
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(UserName, SoupConstants.USER_NAME_LENGTH)));
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(Password, SoupConstants.PASSWORD_LENGTH)));
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(Session, SoupConstants.SESSION_LENGTH)));
            buffer.Put(Encoding.ASCII.GetBytes(Strings.GetPaddedString(SequenceNumber, SoupConstants.SEQUENCE_NUMBER_LENGTH)));
            return buffer.Position - position;
        }
        /// <summary>
        /// Reads the login message from the specified ByteBuffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns>The number of bytes read.</returns>
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
