using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Threading.Tasks;

namespace MMEMessages.Listeners
{
    public class ConnectionListener : IConnectionListener
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly TaskCompletionSource<bool> _loginCompletionSource;
        private readonly TaskCompletionSource<bool> _connectionEstablishedTcs;

        public ConnectionListener(string userName, string password,
                                  TaskCompletionSource<bool> connectionEstablishedTcs,
                                  TaskCompletionSource<bool> loginCompletionSource)
        {
            _userName = userName;
            _password = password;
            _connectionEstablishedTcs = connectionEstablishedTcs;
            _loginCompletionSource = loginCompletionSource;
        }

        public void OnConnectionEstablished(IConnection connection)
        {
            Console.WriteLine($"TCP Connection established. Ready to send login.");
            _connectionEstablishedTcs.TrySetResult(true);
        }

        public void OnConnectionClosed(IConnection connection)
        {
            Console.WriteLine("Connection closed.");
        }

        public void OnConnectionRejected(IConnection connection, char reason)
        {
            Console.WriteLine($"Connection rejected. Reason: {reason}");
            _loginCompletionSource.TrySetResult(false); // Indicate login failure
        }

        public void OnConnectionError(IConnection connection, string errorMessage)
        {
            Console.WriteLine("Connection error occurred:");
            Console.WriteLine($"- {errorMessage}");
            Console.WriteLine("Please check your network connection, server address, and credentials, then try again.");
            _loginCompletionSource.TrySetException(new Exception(errorMessage)); // Propagate exception
        }

        public void OnDataReceived(IConnection connection, ByteBuffer data, long sequenceNumber)
        {
            Console.WriteLine($"Data received. SequenceNumber: {sequenceNumber}, Data Length: {data.Remaining}");

            // Handle data based on message type
            // For example, after login, check for TYPE_ACCEPT or TYPE_REJECT
            // Assuming that TYPE_ACCEPT is handled in SoupConnection.ReceiveData
        }

        public void OnHeartbeat(IConnection connection)
        {
            Console.WriteLine("Heartbeat received.");
        }
    }
}
