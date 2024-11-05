using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Interfaces
{
    public interface IConnectionListener
    {
        void OnDataReceived(IConnection connection, ByteBuffer data, long sequenceNumber);
        void OnConnectionEstablished(IConnection connection);
        void OnConnectionClosed(IConnection connection);
        void OnConnectionRejected(IConnection connection, char reason);
        void OnConnectionError(IConnection connection, string errorMessage);
        void OnHeartbeat(IConnection connection);
    }
}
