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
        private readonly ByteBuffer _dataReceiveBuffer = ByteBuffer.Allocate(1536 * 4 - 22);
        private readonly ByteBuffer _headerSendBuffer = ByteBuffer.Allocate(3);
        private readonly ByteBuffer _dataSendBuffer = ByteBuffer.Allocate(1536 - 22);

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

        public int Id => _connectionId;

        public bool IsConnected => _tcpClient != null && _tcpClient.Connected;

        public bool IsLoggedIn => _loggedIn;

        public bool IsClosed => _closed;

        public string Session => _session;

        public long SequenceNumber => _sequenceNumber;

        public void SetByteOrder(ByteOrder payloadByteOrder)
        {
            _payloadByteOrder = payloadByteOrder;
            _dataSendBuffer.Order = payloadByteOrder;
            _dataReceiveBuffer.Order = payloadByteOrder;
        }

        public void SetTrace(bool trace)
        {
            _trace = trace;
        }

        public void Login(string userName, string password)
        {
            Login(userName, password, "", 1);
        }

        public void Login(string userName, string password, long sequenceNumber)
        {
            Login(userName, password, "", sequenceNumber);
        }

        public void Login(string userName, string password, string session, long sequenceNumber)
        {
            _credentials = new Credentials(userName, password);
            Login(_credentials, session, sequenceNumber);
        }

        public void Login(Credentials credentials)
        {
            Login(credentials, "", 1);
        }

        public void Login(Credentials credentials, long sequenceNumber)
        {
            Login(credentials, "", sequenceNumber);
        }

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

        public void SendMessage(IByteMessage message)
        {
            SendData(SoupConstants.TYPE_UNSEQ, message);
        }

        public void SetUserData(object userData)
        {
            _userData = userData;
        }

        public object GetUserData()
        {
            return _userData;
        }

        public void SendHeartbeat()
        {
            try
            {
                if (_loggedIn)
                {
                    SendData(SoupConstants.TYPE_CLI_HEARTBEAT, null);

                    if (_trace)
                    {
                        Console.WriteLine($"SoupConnection #{_connectionId} sent heartbeat");
                    }

                    _listener?.OnHeartbeat(this);
                }
            }
            catch (Exception)
            {
                _listener?.OnConnectionError(this, "Could not send heartbeat, connection will be closed");
                Close();
            }
        }

        public void ReceiveData()
        {
            try
            {
                if (!IsConnected)
                {
                    Connect();
                }

                if (_networkStream.DataAvailable)
                {
                    // Read header
                    if (_headerReceiveBuffer.HasRemaining)
                    {
                        var headerBytes = new byte[_headerReceiveBuffer.Remaining];
                        int bytesRead = _networkStream.Read(headerBytes, 0, headerBytes.Length);
                        if (bytesRead == 0)
                        {
                            throw new IOException("Reached EOF");
                        }
                        _headerReceiveBuffer.Put(headerBytes, 0, bytesRead);
                        if (_headerReceiveBuffer.HasRemaining)
                        {
                            return;
                        }
                        _headerReceiveBuffer.Flip();
                    }

                    // Read data
                    var soupHeader = new SoupHeader();
                    soupHeader.Read(_headerReceiveBuffer);
                    int payloadLength = soupHeader.PayloadLength;

                    if (_dataReceiveBuffer.Remaining < payloadLength)
                    {
                        _dataReceiveBuffer.Limit = payloadLength;
                    }

                    if (_dataReceiveBuffer.HasRemaining)
                    {
                        var dataBytes = new byte[_dataReceiveBuffer.Remaining];
                        int bytesRead = _networkStream.Read(dataBytes, 0, dataBytes.Length);
                        if (bytesRead == 0)
                        {
                            throw new IOException("Reached EOF");
                        }
                        _dataReceiveBuffer.Put(dataBytes, 0, bytesRead);
                        if (_dataReceiveBuffer.HasRemaining)
                        {
                            return;
                        }
                        _dataReceiveBuffer.Flip();
                    }

                    // Process message
                    switch (soupHeader.Type)
                    {
                        case SoupConstants.TYPE_ACCEPT:
                            var soupAccept = new SoupAccept();
                            soupAccept.Read(_dataReceiveBuffer);
                            _session = soupAccept.Session;
                            _sequenceNumber = soupAccept.SequenceNumber;
                            _loggedIn = true;
                            _listener?.OnConnectionEstablished(this);
                            if (_trace)
                            {
                                Console.WriteLine("Connection Established");
                            }
                            break;
                        case SoupConstants.TYPE_SEQ:
                            if (_trace)
                            {
                                Console.WriteLine($"Received {_dataReceiveBuffer.Limit} bytes of data with sequence number {_sequenceNumber} for {this}");
                            }
                            _listener?.OnDataReceived(this, _dataReceiveBuffer.AsReadOnlyBuffer(), _sequenceNumber);
                            _sequenceNumber++;
                            break;
                        case SoupConstants.TYPE_SRV_HEARTBEAT:
                            if (_trace)
                            {
                                Console.WriteLine("Received Server Heartbeat");
                            }
                            _listener?.OnHeartbeat(this);
                            break;
                        case SoupConstants.TYPE_EOS:
                            if (_trace)
                            {
                                Console.WriteLine("Connection Closed");
                            }
                            _listener?.OnConnectionClosed(this);
                            Close();
                            break;
                        case SoupConstants.TYPE_REJECT:
                            var soupReject = new SoupReject();
                            soupReject.Read(_dataReceiveBuffer);
                            if (_trace)
                            {
                                Console.WriteLine($"Connection Rejected: {soupReject.Reason}");
                            }
                            _listener?.OnConnectionRejected(this, soupReject.Reason);
                            Close();
                            break;
                        default:
                            if (_trace)
                            {
                                Console.WriteLine($"Got message of type {soupHeader.Type}");
                            }
                            break;
                    }

                    // Clear buffers
                    _headerReceiveBuffer.Clear();
                    _dataReceiveBuffer.Clear();
                }
            }
            catch (Exception e)
            {
                _listener?.OnConnectionError(this, e.Message);
                Close();
            }
        }

        private void Connect()
        {
            if (_trace)
            {
                Console.WriteLine("SoupConnection.Connect");
            }

            _tcpClient = new TcpClient();
            _tcpClient.Connect(_host, _port);
            _networkStream = _tcpClient.GetStream();
        }

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
                soupHeader.Write(_headerSendBuffer);
                _headerSendBuffer.Flip();

                var buffers = new List<ArraySegment<byte>>
                {
                    new ArraySegment<byte>(_headerSendBuffer.ToArray()),
                    new ArraySegment<byte>(_dataSendBuffer.ToArray())
                };

                foreach (var buffer in buffers)
                {
                    _networkStream.Write(buffer.Array, buffer.Offset, buffer.Count);
                }
            }
        }

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
