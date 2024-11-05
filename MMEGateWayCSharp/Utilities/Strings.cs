using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Utilities
{
    /// <summary>
    /// Provides utility methods for string manipulation.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Pads the input string to the specified length with spaces.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="length">The desired length.</param>
        /// <returns>The padded string.</returns>
        public static string GetPaddedString(string str, int length)
        {
            return GetPaddedString(str, length, ' ', false);
        }
        /// <summary>
        /// Pads the input string to the specified length with the given character.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="length">The desired length.</param>
        /// <param name="token">The padding character.</param>
        public static string GetPaddedString(string str, int length, char token)
        {
            return GetPaddedString(str, length, token, false);
        }
        /// <summary>
        /// Pads the input string to the specified length with spaces, optionally prepending the padding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="length">The desired length.</param>
        /// <param name="prepend">True to prepend padding; false to append.</param>
        /// <returns>The padded string.</returns>
        public static string GetPaddedString(string str, int length, bool prepend)
        {
            return GetPaddedString(str, length, ' ', prepend);
        }
        /// <summary>
        /// Pads the input string to the specified length with the given character, optionally prepending the padding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="length">The desired length.</param>
        /// <param name="token">The padding character.</param>
        /// <param name="prepend">True to prepend padding; false to append.</param>
        /// <returns>The padded string.</returns>
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
        /// <summary>
        /// Pads the integer number to the specified length with spaces, prepending the padding.
        /// </summary>
        /// <param name="number">The integer number.</param>
        /// <param name="length">The desired length.</param>
        /// <returns>The padded string representation of the number.</returns>
        public static string GetPaddedString(int number, int length)
        {
            return GetPaddedString(number.ToString(), length, ' ', true);
        }
        /// <summary>
        /// Pads the long number to the specified length with spaces, prepending the padding.
        /// </summary>
        /// <param name="number">The long number.</param>
        /// <param name="length">The desired length.</param>
        /// <returns>The padded string representation of the number.</returns>
        public static string GetPaddedString(long number, int length)
        {
            return GetPaddedString(number.ToString(), length, ' ', true);
        }
    }
}
