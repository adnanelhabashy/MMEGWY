using MMEGateWayCSharp.Models;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Interfaces
{
    /// <summary>
    /// Represents a connection in the Soup protocol.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Gets the unique identifier of the connection.
        /// </summary>
        int Id { get; }
        /// <summary>
        /// Gets a value indicating whether the connection is established.
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// Gets a value indicating whether the connection is logged in.
        /// </summary>
        bool IsLoggedIn { get; }
        /// <summary>
        /// Gets a value indicating whether the connection is closed.
        /// </summary>
        bool IsClosed { get; }
        /// <summary>
        /// Gets the session identifier associated with the connection.
        /// </summary>
        string Session { get; }
        /// <summary>
        /// Gets the current sequence number for the connection.
        /// </summary>
        long SequenceNumber { get; }
        /// <summary>
        /// Sets the byte order for the payload.
        /// </summary>
        /// <param name="payloadByteOrder">The byte order to set.</param>
        void SetByteOrder(ByteOrder payloadByteOrder);
        /// <summary>
        /// Logs in to the server using the specified username and password.
        /// </summary>
        /// <param name="userName">The username to log in with.</param>
        /// <param name="password">The password to log in with.</param>
        void Login(string userName, string password);
        /// <summary>
        /// Logs in to the server using the specified username, password, and sequence number.
        /// </summary>
        /// <param name="userName">The username to log in with.</param>
        /// <param name="password">The password to log in with.</param>
        /// <param name="sequenceNumber">The sequence number to start with.</param>
        void Login(string userName, string password, long sequenceNumber);
        /// <summary>
        /// Logs in to the server using the specified username, password, session, and sequence number.
        /// </summary>
        /// <param name="userName">The username to log in with.</param>
        /// <param name="password">The password to log in with.</param>
        /// <param name="session">The session identifier to use.</param>
        /// <param name="sequenceNumber">The sequence number to start with.</param>
        void Login(string userName, string password, string session, long sequenceNumber);
        /// <summary>
        /// Logs in to the server using the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials containing username and password.</param>
        void Login(Credentials credentials);
        /// <summary>
        /// Logs in to the server using the specified credentials and sequence number.
        /// </summary>
        /// <param name="credentials">The credentials containing username and password.</param>
        /// <param name="sequenceNumber">The sequence number to start with.</param>
        void Login(Credentials credentials, long sequenceNumber);
        /// <summary>
        /// Logs in to the server using the specified credentials, session, and sequence number.
        /// </summary>
        /// <param name="credentials">The credentials containing username and password.</param>
        /// <param name="session">The session identifier to use.</param>
        /// <param name="sequenceNumber">The sequence number to start with.</param>
        void Login(Credentials credentials, string session, long sequenceNumber);
        /// <summary>
        /// Logs out from the server and closes the connection.
        /// </summary>
        void Logout();
        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Close();
        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void SendMessage(IByteMessage message);
        /// <summary>
        /// Sets user-defined data associated with the connection.
        /// </summary>
        /// <param name="userData">An object containing user data.</param>
        void SetUserData(object userData);
        /// <summary>
        /// Gets the user-defined data associated with the connection.
        /// </summary>
        /// <returns>An object containing user data.</returns>
        object GetUserData();
        /// <summary>
        /// Enables or disables tracing of connection activities.
        /// </summary>
        /// <param name="trace">True to enable tracing; otherwise, false.</param>
        void SetTrace(bool trace);
    }
}
