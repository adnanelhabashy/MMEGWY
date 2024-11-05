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
        public void OnConnectionEstablished(IConnection connection)
        {
            Console.WriteLine($"Connection established. Session: {connection.Session}, SequenceNumber: {connection.SequenceNumber}");
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
            Console.WriteLine($"Connection error: {errorMessage}");
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
