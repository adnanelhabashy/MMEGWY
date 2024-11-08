using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEMessages.Listeners
{
    public class ConnectionListener : IConnectionListener
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly TaskCompletionSource<bool> _loginCompletionSource;

        public ConnectionListener(string userName, string password, TaskCompletionSource<bool> loginCompletionSource)
        {
            _userName = userName;
            _password = password;
            _loginCompletionSource = loginCompletionSource;
        }

        public void OnConnectionEstablished(IConnection connection)
        {
            Console.WriteLine($"TCP Connection established. Ready to send login.");
            // Do not initiate login here to avoid circular dependency
        }

        public void OnConnectionClosed(IConnection connection)
        {
            Console.WriteLine("Connection closed.");
        }

        public void OnConnectionRejected(IConnection connection, char reason)
        {
            Console.WriteLine($"Connection rejected. Reason: {reason}");
        }

        public void OnConnectionError(IConnection connection, string errorMessage)
        {
            Console.WriteLine("Connection error occurred:");
            Console.WriteLine($"- {errorMessage}");
            Console.WriteLine("Please check your network connection, server address, and credentials, then try again.");
        }

        public void OnDataReceived(IConnection connection, ByteBuffer data, long sequenceNumber)
        {
            Console.WriteLine($"Data received. SequenceNumber: {sequenceNumber}, Data Length: {data.Remaining}");

            // Here you can parse the data received based on your protocol
            // For demonstration, let's assume we receive a MmeCommit message

            var mmeCommit = new MMEMessages.Models.MmeCommit();
            mmeCommit.Read(data);

            Console.WriteLine($"Received MmeCommit: MessageGroup={mmeCommit.MessageGroup}, MessageId={mmeCommit.MessageId}");
        }

        public void OnHeartbeat(IConnection connection)
        {
            Console.WriteLine("Heartbeat received.");
        }
    }
}
