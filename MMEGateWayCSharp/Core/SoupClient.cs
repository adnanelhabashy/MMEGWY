using MMEGateWayCSharp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Core
{
    public sealed class SoupClient
    {
        private const int SELECT_TIMEOUT_MS = 100;

        private readonly List<SoupConnection> _connections = new List<SoupConnection>();
        private readonly BlockingCollection<RegisterInfo> _registerQueue = new BlockingCollection<RegisterInfo>();
        private volatile bool _stopThread = false;

        private SoupClient()
        {
        }

        public static SoupClient Create()
        {
            var soupClient = new SoupClient();
            soupClient.Init();
            return soupClient;
        }

        private void Init()
        {
            var selectorThread = new Thread(SelectorThread);
            selectorThread.Start();
        }

        public void Close()
        {
            _stopThread = true;
            foreach (var connection in _connections)
            {
                connection.Close();
            }
            Console.WriteLine("Closing down SoupClient");
        }

        public IConnection CreateConnection(string node, int port)
        {
            return CreateConnection(node, port, null);
        }

        public IConnection CreateConnection(string node, int port, IConnectionListener listener)
        {
            return CreateConnection(node, port, listener, null);
        }

        public IConnection CreateConnection(string node, int port, IConnectionListener listener, object userData)
        {
            var connection = new SoupConnection(node, port, listener);
            connection.SetUserData(userData);

            _connections.Add(connection);

            var registerInfo = new RegisterInfo(connection);
            _registerQueue.Add(registerInfo);

            return connection;
        }

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

        private class RegisterInfo
        {
            public SoupConnection Connection { get; }

            public RegisterInfo(SoupConnection connection)
            {
                Connection = connection;
            }
        }
    }
}
