using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Core
{
    public static class SoupConstants
    {
        public const int USER_NAME_LENGTH = 6;
        public const int PASSWORD_LENGTH = 10;
        public const int SESSION_LENGTH = 10;
        public const int SEQUENCE_NUMBER_LENGTH = 20;

        public const byte TYPE_REJECT = (byte)'J';
        public const byte TYPE_SEQ = (byte)'S';
        public const byte TYPE_UNSEQ = (byte)'U';
        public const byte TYPE_ACCEPT = (byte)'A';
        public const byte TYPE_DEBUG = (byte)'+';
        public const byte TYPE_SRV_HEARTBEAT = (byte)'H';
        public const byte TYPE_EOS = (byte)'Z';
        public const byte TYPE_LOGIN = (byte)'L';
        public const byte TYPE_CLI_HEARTBEAT = (byte)'R';
        public const byte TYPE_LOGOUT = (byte)'O';
    }
}
