using MMEGateWayCSharp.Exceptions;
using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Models;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Core
{
    /// <summary>
    /// Represents a Soup TCP connection that implements the <see cref="IConnection"/> interface.
    /// </summary>
    public class SoupConnection : IConnection
    {
        private static int CONNECTION_ID_COUNTER = 1;
        private static readonly object ConnectionIdLock = new object();

        private readonly int _connectionId;
        private readonly string _host;
        private readonly int _port;
        private readonly IConnectionListener _listener;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private Credentials _credentials;
        private string _session;
        private long _sequenceNumber;
        private object _userData;
        private bool _trace = false;
        private ByteOrder _payloadByteOrder = ByteOrder.BigEndian;
        private bool _loggedIn = false;
        private bool _closed = false;
        private readonly object _lock = new object();

        // Buffers
        private readonly ByteBuffer _headerReceiveBuffer = ByteBuffer.Allocate(3);
        private  ByteBuffer _dataReceiveBuffer = ByteBuffer.Allocate(1536 * 4 - 22);
        private readonly ByteBuffer _headerSendBuffer = ByteBuffer.Allocate(3);
        private readonly ByteBuffer _dataSendBuffer = ByteBuffer.Allocate(1536 - 22);

        /// <summary>
        /// Initializes a new instance of the <see cref="SoupConnection"/> class.
        /// </summary>
        /// <param name="host">The hostname or IP address of the server.</param>
        /// <param name="port">The port number to connect to.</param>
        /// <param name="listener">An optional connection listener.</param>
        public SoupConnection(string host, int port, IConnectionListener listener)
        {
            lock (ConnectionIdLock)
            {
                _connectionId = CONNECTION_ID_COUNTER++;
            }

            _host = host;
            _port = port;
            _listener = listener;
        }

        /// <summary>
        /// Gets the unique identifier of the connection.
        /// </summary>
        public int Id => _connectionId;

        /// <summary>
        /// Gets a value indicating whether the connection is established.
        /// </summary>
        public bool IsConnected => _tcpClient != null && _tcpClient.Connected;

        /// <summary>
        /// Gets a value indicating whether the connection is logged in.
        /// </summary>
        public bool IsLoggedIn => _loggedIn;

        /// <summary>
        /// Gets a value indicating whether the connection is closed.
        /// </summary>
        public bool IsClosed => _closed;

        /// <summary>
        /// Gets the session identifier associated with the connection.
        /// </summary>
        public string Session => _session;

        /// <summary>
        /// Gets the current sequence number for the connection.
        /// </summary>
        public long SequenceNumber => _sequenceNumber;

        /// <summary>
        /// Sets the byte order for the payload.
        /// </summary>
        /// <param name="payloadByteOrder">The byte order to set.</param>
        public void SetByteOrder(ByteOrder payloadByteOrder)
        {
            _payloadByteOrder = payloadByteOrder;
            _dataSendBuffer.Order = payloadByteOrder;
            _dataReceiveBuffer.Order = payloadByteOrder;
        }

        /// <summary>
        /// Enables or disables tracing of connection operations.
        /// </summary>
        /// <param name="trace">If set to <c>true</c>, tracing is enabled.</param>
        public void SetTrace(bool trace)
        {
            _trace = trace;
        }

        /// <summary>
        /// Logs in to the connection with the specified username and password.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        public void Login(string userName, string password)
        {
            Login(userName, password, "", 1);
        }

        /// <summary>
        /// Logs in to the connection with the specified username, password, and sequence number.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="sequenceNumber">The starting sequence number.</param>
        public void Login(string userName, string password, long sequenceNumber)
        {
            Login(userName, password, "", sequenceNumber);
        }

        /// <summary>
        /// Logs in to the connection with the specified username, password, session, and sequence number.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="session">The session identifier.</param>
        /// <param name="sequenceNumber">The starting sequence number.</param>
        public void Login(string userName, string password, string session, long sequenceNumber)
        {
            _credentials = new Credentials(userName, password);
            Login(_credentials, session, sequenceNumber);
        }

        /// <summary>
        /// Logs in to the connection with the specified credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        public void Login(Credentials credentials)
        {
            Login(credentials, "", 1);
        }

        /// <summary>
        /// Logs in to the connection with the specified credentials and sequence number.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="sequenceNumber">The starting sequence number.</param>
        public void Login(Credentials credentials, long sequenceNumber)
        {
            Login(credentials, "", sequenceNumber);
        }

        /// <summary>
        /// Logs in to the connection with the specified credentials, session, and sequence number.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="session">The session identifier.</param>
        /// <param name="sequenceNumber">The starting sequence number.</param>
        public void Login(Credentials credentials, string session, long sequenceNumber)
        {
            if (_trace)
            {
                Console.WriteLine("SoupConnection.Login");
            }

            if (IsClosed)
            {
                throw new SoupException("Connection closed, create new connection before logging in");
            }
            if (!IsConnected)
            {
                throw new SoupException("Connection not established");
            }
            if (IsLoggedIn)
            {
                throw new SoupException("Connection already logged in");
            }

            ValidateCredentials(credentials);
            _credentials = credentials;

            var soupLogin = new SoupLogin(credentials.UserName, credentials.Password, session, sequenceNumber);
            SendData(SoupConstants.TYPE_LOGIN, soupLogin);
        }

        /// <summary>
        /// Logs out from the connection and closes it.
        /// </summary>
        public void Logout()
        {
            if (_trace)
            {
                Console.WriteLine("SoupConnection.Logout");
            }

            try
            {
                if (_loggedIn)
                {
                    SendData(SoupConstants.TYPE_LOGOUT, null);
                    _loggedIn = false;
                }
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            if (_trace)
            {
                Console.WriteLine("SoupConnection.Close");
            }

            lock (_lock)
            {
                if (!_closed)
                {
                    _networkStream?.Close();
                    _tcpClient?.Close();
                    _loggedIn = false;
                    _closed = true;

                    _listener?.OnConnectionClosed(this);
                }
            }
        }

        /// <summary>
        /// Sends a message over the connection.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessage(IByteMessage message)
        {
            SendData(SoupConstants.TYPE_UNSEQ, message);
        }

        /// <summary>
        /// Sets user-defined data associated with the connection.
        /// </summary>
        /// <param name="userData">The user data to associate.</param>
        public void SetUserData(object userData)
        {
            _userData = userData;
        }

        /// <summary>
        /// Gets the user-defined data associated with the connection.
        /// </summary>
        /// <returns>The user data associated with the connection.</returns>
        public object GetUserData()
        {
            return _userData;
        }

        /// <summary>
        /// Sends a heartbeat message to the server.
        /// </summary>
        public void SendHeartbeat()
        {
            try
            {
                if (_loggedIn)
                {
                    SendData(SoupConstants.TYPE_CLI_HEARTBEAT, null);

                    if (_trace)
                    {
                        Console.WriteLine("SoupConnection sent heartbeat");
                    }

                    _listener?.OnHeartbeat(this);
                }
            }
            catch (Exception e)
            {
                _listener?.OnConnectionError(this, $"Could not send heartbeat: {e.Message}");
                Close();
            }
        }

        /// <summary>
        /// Receives data from the server.
        /// </summary>
        public void ReceiveData()
        {
            try
            {
                if (!IsConnected)
                {
                    Connect();
                    Console.WriteLine("Connected to server, notifying listener.");
                    _listener?.OnConnectionEstablished(this);
                }

                if (_networkStream.DataAvailable)
                {
                    // Read Header if not fully read
                    if (_headerReceiveBuffer.HasRemaining)
                    {
                        byte[] headerBytes = new byte[_headerReceiveBuffer.Remaining];
                        int bytesRead = _networkStream.Read(headerBytes, 0, headerBytes.Length);
                        if (bytesRead == 0)
                        {
                            throw new IOException("Reached EOF");
                        }
                        _headerReceiveBuffer.Put(headerBytes, 0, bytesRead);
                        Console.WriteLine("Received Header Bytes: {HeaderBytes}", BitConverter.ToString(headerBytes));

                        if (_headerReceiveBuffer.HasRemaining)
                        {
                            return; // Wait for more data
                        }
                        _headerReceiveBuffer.Flip();
                    }

                    // Parse Header
                    var soupHeader = new SoupHeader();
                    soupHeader.Read(_headerReceiveBuffer);
                    Console.WriteLine("Parsed SoupHeader: Length={Length}, Type={Type}", soupHeader.Length, (char)soupHeader.Type);

                    int payloadLength = soupHeader.PayloadLength;

                    // Adjust Data Buffer if needed
                    if (_dataReceiveBuffer.Remaining < payloadLength)
                    {
                        _dataReceiveBuffer = ByteBuffer.Allocate(payloadLength);
                        Console.WriteLine("Allocated new data receive buffer with size {Size}", payloadLength);
                    }

                    // Read Payload
                    if (_dataReceiveBuffer.HasRemaining)
                    {
                        byte[] dataBytes = new byte[_dataReceiveBuffer.Remaining];
                        int bytesRead = _networkStream.Read(dataBytes, 0, dataBytes.Length);
                        if (bytesRead == 0)
                        {
                            throw new IOException("Reached EOF");
                        }
                        _dataReceiveBuffer.Put(dataBytes, 0, bytesRead);
                        Console.WriteLine("Received Data Bytes: {DataBytes}", BitConverter.ToString(dataBytes));

                        if (_dataReceiveBuffer.HasRemaining)
                        {
                            return; // Wait for more data
                        }
                        _dataReceiveBuffer.Flip();
                    }

                    // Handle Message Based on Type
                    switch (soupHeader.Type)
                    {
                        case SoupConstants.TYPE_ACCEPT:
                            var soupAccept = new SoupAccept();
                            soupAccept.Read(_dataReceiveBuffer);
                            _session = soupAccept.Session.Trim();
                            _sequenceNumber = soupAccept.SequenceNumber;
                            _loggedIn = true;
                            Console.WriteLine("Login accepted. Session: {Session}, SequenceNumber: {SequenceNumber}", _session, _sequenceNumber);
                            _listener?.OnConnectionEstablished(this);
                            break;

                        case SoupConstants.TYPE_REJECT:
                            var soupReject = new SoupReject();
                            soupReject.Read(_dataReceiveBuffer);
                            Console.WriteLine("Login rejected. Reason: {Reason}", soupReject.Reason);
                            _listener?.OnConnectionRejected(this, soupReject.Reason);
                            Close();
                            break;

                        case SoupConstants.TYPE_SRV_HEARTBEAT:
                            Console.WriteLine("Received Server Heartbeat");
                            _listener?.OnHeartbeat(this);
                            break;

                        case SoupConstants.TYPE_EOS:
                            Console.WriteLine("Connection Closed by Server");
                            _listener?.OnConnectionClosed(this);
                            Close();
                            break;

                        default:
                            Console.WriteLine("Received unknown message type: {Type}", (char)soupHeader.Type);
                            break;
                    }

                    // Clear Buffers for Next Message
                    _headerReceiveBuffer.Clear();
                    _dataReceiveBuffer.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e+ " ReceiveData exception: {Message} "+ e.Message);
                _listener?.OnConnectionError(this, e.Message);
                Close();
            }
        }

        /// <summary>
        /// Establishes the TCP connection to the server.
        /// </summary>
        private void Connect()
        {
            if (_trace)
            {
                Console.WriteLine("SoupConnection.Connect");
            }

            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(_host, _port);
                _networkStream = _tcpClient.GetStream();
                if (_trace)
                {
                    Console.WriteLine("Successfully connected to {Host}:{Port}", _host, _port);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex+" Connect failed: {Message} "+ ex.Message);
                throw; // Re-throw to be handled in ReceiveData
            }
        }

        /// <summary>
        /// Sends data to the server.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="payload">The message payload.</param>
        private void SendData(byte type, IByteMessage payload)
        {
            if (_trace)
            {
                Console.WriteLine("SoupConnection.SendData");
            }

            if (IsConnected)
            {
                int dataLength = 0;

                _dataSendBuffer.Clear();
                if (payload != null)
                {
                    dataLength = payload.Write(_dataSendBuffer);
                }
                _dataSendBuffer.Flip();

                _headerSendBuffer.Clear();
                var soupHeader = new SoupHeader();
                soupHeader.Length = dataLength + 1;
                soupHeader.Type = type;

                // Ensure the header buffer is set to BigEndian
                _headerSendBuffer.Order = ByteOrder.BigEndian;
                soupHeader.Write(_headerSendBuffer);
                _headerSendBuffer.Flip();

                var headerBytes = _headerSendBuffer.ToArray();
                var dataBytes = _dataSendBuffer.ToArray();

                Console.WriteLine("Sending Header: {HeaderBytes}", BitConverter.ToString(headerBytes));
                Console.WriteLine("Sending Data: {DataBytes}", BitConverter.ToString(dataBytes));

                var buffers = new List<ArraySegment<byte>>
                {
                    new ArraySegment<byte>(headerBytes),
                    new ArraySegment<byte>(dataBytes)
                };

                foreach (var buffer in buffers)
                {
                    _networkStream.Write(buffer.Array, buffer.Offset, buffer.Count);
                }
                Console.WriteLine("Data sent successfully.");
            }
            else
            {
                Console.WriteLine("Attempted to send data while not connected.");
            }
        }

        /// <summary>
        /// Validates the provided credentials.
        /// </summary>
        /// <param name="credentials">The credentials to validate.</param>
        /// <exception cref="SoupException">Thrown when the credentials are invalid.</exception>
        private void ValidateCredentials(Credentials credentials)
        {
            if (credentials == null)
            {
                throw new SoupException("The credentials cannot be null");
            }
            if (credentials.UserName == null)
            {
                throw new SoupException("The user name cannot be null");
            }
            if (credentials.UserName.Length > SoupConstants.USER_NAME_LENGTH)
            {
                throw new SoupException($"The user name length must not be longer than {SoupConstants.USER_NAME_LENGTH}");
            }
            if (credentials.Password == null)
            {
                throw new SoupException("The password cannot be null");
            }
            if (credentials.Password.Length > SoupConstants.PASSWORD_LENGTH)
            {
                throw new SoupException($"The password length must not be longer than {SoupConstants.PASSWORD_LENGTH}");
            }
        }

        /// <summary>
        /// Returns a string representation of the connection.
        /// </summary>
        /// <returns>A string representing the connection state.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Connection #").Append(_connectionId).Append(" [");
            if (IsLoggedIn)
            {
                sb.Append("LoggedIn");
            }
            else if (IsConnected)
            {
                sb.Append("Connected");
            }
            else if (IsClosed)
            {
                sb.Append("Closed");
            }
            else
            {
                sb.Append("Unknown");
            }
            sb.Append("] ");
            if (_credentials != null)
            {
                sb.Append(_credentials.UserName).Append(":").Append(_credentials.Password).Append("@");
            }
            else
            {
                sb.Append("<N/A>@");
            }
            sb.Append(_host).Append(":").Append(_port);
            return sb.ToString();
        }
    }
}
