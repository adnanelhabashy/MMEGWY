using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Core
{
    /// <summary>
    /// Provides constants used in the Soup protocol.
    /// </summary>
    public static class SoupConstants
    {

        /// <summary>
        /// The maximum length of the username field.
        /// </summary>
        public const int USER_NAME_LENGTH = 6;
        /// <summary>
        /// The maximum length of the password field.
        /// </summary>
        public const int PASSWORD_LENGTH = 10;
        /// <summary>
        /// The maximum length of the session field.
        /// </summary>
        public const int SESSION_LENGTH = 10;
        /// <summary>
        /// The maximum length of the sequence number field.
        /// </summary>
        public const int SEQUENCE_NUMBER_LENGTH = 20;
        /// <summary>
        /// Message type for rejection.
        /// </summary>
        public const byte TYPE_REJECT = (byte)'J';
        /// <summary>
        /// Message type for sequenced data.
        /// </summary>
        public const byte TYPE_SEQ = (byte)'S';
        /// <summary>
        /// Message type for unsequenced data.
        /// </summary>
        public const byte TYPE_UNSEQ = (byte)'U';
        /// <summary>
        /// Message type for acceptance.
        /// </summary>
        public const byte TYPE_ACCEPT = (byte)'A';
        /// <summary>
        /// Message type for debug messages.
        /// </summary>
        public const byte TYPE_DEBUG = (byte)'+';
        /// <summary>
        /// Message type for server heartbeat.
        /// </summary>
        public const byte TYPE_SRV_HEARTBEAT = (byte)'H';
        /// <summary>
        /// Message type indicating end of session.
        /// </summary>
        public const byte TYPE_EOS = (byte)'Z';
        /// <summary>
        /// Message type for login.
        /// </summary>
        public const byte TYPE_LOGIN = (byte)'L';
        /// <summary>
        /// Message type for client heartbeat.
        /// </summary>
        public const byte TYPE_CLI_HEARTBEAT = (byte)'R';
        /// <summary>
        /// Message type for logout.
        /// </summary>
        public const byte TYPE_LOGOUT = (byte)'O';
    }
}
