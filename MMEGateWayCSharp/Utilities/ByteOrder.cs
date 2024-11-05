using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Utilities
{
    /// <summary>
    /// Represents the byte order (endianness) used in byte buffers.
    /// </summary>
    public enum ByteOrder
    {
        /// <summary>
        /// Big-endian byte order.
        /// </summary>
        BigEndian,
        /// <summary>
        /// Little-endian byte order.
        /// </summary>
        LittleEndian
    }
}
