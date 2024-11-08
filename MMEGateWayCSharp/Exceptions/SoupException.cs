using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Exceptions
{
    /// <summary>
    /// Represents errors that occur during Soup protocol operations.
    /// </summary>
    public class SoupException : IOException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoupException"/> class.
        /// </summary>
        public SoupException() : base()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SoupException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SoupException(string message) : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SoupException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>

        public SoupException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class BufferUnderflowException : Exception
    {
        public BufferUnderflowException() { }

        public BufferUnderflowException(string message) : base(message) { }

        public BufferUnderflowException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BufferOverflowException : Exception
    {
        public BufferOverflowException() { }

        public BufferOverflowException(string message) : base(message) { }

        public BufferOverflowException(string message, Exception innerException) : base(message, innerException) { }
    }
}
