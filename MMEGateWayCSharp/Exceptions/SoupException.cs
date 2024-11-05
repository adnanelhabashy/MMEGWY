using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Exceptions
{
    public class SoupException : IOException
    {
        public SoupException() : base()
        {
        }

        public SoupException(string message) : base(message)
        {
        }

        public SoupException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
