using MMEGateWayCSharp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Core
{
    /// <summary>
    /// Represents a client that manages Soup connections.
    /// </summary>
    public sealed class SoupClient
    {
        private const int SELECT_TIMEOUT_MS = 100;

        private readonly List<SoupConnection> _connections = new List<SoupConnection>();
        private readonly BlockingCollection<RegisterInfo> _registerQueue = new BlockingCollection<RegisterInfo>();
        private volatile bool _stopThread = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="SoupClient"/> class.
        /// </summary>
        private SoupClient()
        {
        }
        /// <summary>
        /// Creates and initializes a new instance of <see cref="SoupClient"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="SoupClient"/>.</returns>
        public static SoupClient Create()
        {
            var soupClient = new SoupClient();
            soupClient.Init();
            return soupClient;
        }
        /// <summary>
        /// Initializes the SoupClient by starting the selector thread.
        /// </summary>
        private void Init()
        {
            var selectorThread = new Thread(SelectorThread);
            selectorThread.Start();
        }
        /// <summary>
        /// Closes the SoupClient and all its connections.
        /// </summary>
        public void Close()
        {
            _stopThread = true;
            foreach (var connection in _connections)
            {
                connection.Close();
            }
            Console.WriteLine("Closing down SoupClient");
        }

        /// <summary>
        /// Creates a new connection to the specified node and port.
        /// </summary>
        /// <param name="node">The hostname or IP address of the node.</param>
        /// <param name="port">The port number.</param>
        /// <returns>A new <see cref="IConnection"/> instance.</returns>
        public IConnection CreateConnection(string node, int port)
        {
            return CreateConnection(node, port, null);
        }
        /// <summary>
        /// Creates a new connection to the specified node and port with a listener.
        /// </summary>
        /// <param name="node">The hostname or IP address of the node.</param>
        /// <param name="port">The port number.</param>
        /// <param name="listener">An optional connection listener.</param>
        /// <returns>A new <see cref="IConnection"/> instance.</returns>

        public IConnection CreateConnection(string node, int port, IConnectionListener listener)
        {
            return CreateConnection(node, port, listener, null);
        }
        /// <summary>
        /// Creates a new connection to the specified node and port with a listener and user data.
        /// </summary>
        /// <param name="node">The hostname or IP address of the node.</param>
        /// <param name="port">The port number.</param>
        /// <param name="listener">An optional connection listener.</param>
        /// <param name="userData">An optional user data object associated with the connection.</param>
        /// <returns>A new <see cref="IConnection"/> instance.</returns>
        public IConnection CreateConnection(string node, int port, IConnectionListener listener, object userData)
        {
            var connection = new SoupConnection(node, port, listener);
            connection.SetUserData(userData);

            _connections.Add(connection);

            var registerInfo = new RegisterInfo(connection);
            _registerQueue.Add(registerInfo);

            return connection;
        }

        /// <summary>
        /// The thread method that continuously receives data from all connections.
        /// </summary>
        private void SelectorThread()
        {
            while (!_stopThread)
            {
                try
                {
                    foreach (var connection in _connections)
                    {
                        connection.ReceiveData();
                    }

                    Thread.Sleep(SELECT_TIMEOUT_MS);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SelectorThread exception: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Contains registration information for a connection.
        /// </summary>
        private class RegisterInfo
        {
            /// <summary>
            /// Gets the associated <see cref="SoupConnection"/>.
            /// </summary>
            public SoupConnection Connection { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="RegisterInfo"/> class.
            /// </summary>
            /// <param name="connection">The associated connection.</param>
            public RegisterInfo(SoupConnection connection)
            {
                Connection = connection;
            }
        }
    }
}
