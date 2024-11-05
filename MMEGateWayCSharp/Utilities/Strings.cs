using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Utilities
{
    public static class Strings
    {
        public static string GetPaddedString(string str, int length)
        {
            return GetPaddedString(str, length, ' ', false);
        }

        public static string GetPaddedString(string str, int length, char token)
        {
            return GetPaddedString(str, length, token, false);
        }

        public static string GetPaddedString(string str, int length, bool prepend)
        {
            return GetPaddedString(str, length, ' ', prepend);
        }

        public static string GetPaddedString(string str, int length, char token, bool prepend)
        {
            if (str.Length > length)
            {
                return str.Substring(0, length);
            }
            else if (str.Length == length)
            {
                return str;
            }

            if (prepend)
            {
                return new string(token, length - str.Length) + str;
            }
            else
            {
                return str + new string(token, length - str.Length);
            }
        }

        public static string GetPaddedString(int number, int length)
        {
            return GetPaddedString(number.ToString(), length, ' ', true);
        }

        public static string GetPaddedString(long number, int length)
        {
            return GetPaddedString(number.ToString(), length, ' ', true);
        }
    }
}
