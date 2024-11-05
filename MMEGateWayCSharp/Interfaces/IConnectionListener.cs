using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Interfaces
{
    /// <summary>
    /// Defines methods to handle events related to a SOUP protocol connection.
    /// </summary>
    public interface IConnectionListener
    {
        /// <summary>
        /// Invoked when data is received from the connection.
        /// </summary>
        /// <param name="connection">The connection from which data was received.</param>
        /// <param name="data">The received data buffer.</param>
        /// <param name="sequenceNumber">The sequence number of the received data.</param>
        void OnDataReceived(IConnection connection, ByteBuffer data, long sequenceNumber);
        /// <summary>
        /// Invoked when the connection has been successfully established.
        /// </summary>
        /// <param name="connection">The established connection.</param>
        void OnConnectionEstablished(IConnection connection);
        /// <summary>
        /// Invoked when the connection has been closed.
        /// </summary>
        /// <param name="connection">The connection that was closed.</param>
        void OnConnectionClosed(IConnection connection);
        /// <summary>
        /// Invoked when the connection is rejected by the server.
        /// </summary>
        /// <param name="connection">The connection that was rejected.</param>
        /// <param name="reason">The reason code for rejection.</param>
        void OnConnectionRejected(IConnection connection, char reason);
        /// <summary>
        /// Invoked when an error occurs on the connection.
        /// </summary>
        /// <param name="connection">The connection where the error occurred.</param>
        /// <param name="errorMessage">The error message.</param>
        void OnConnectionError(IConnection connection, string errorMessage);
        /// <summary>
        /// Invoked when a heartbeat message is received from the server.
        /// </summary>
        /// <param name="connection">The connection that received the heartbeat.</param>

        void OnHeartbeat(IConnection connection);
    }
}
