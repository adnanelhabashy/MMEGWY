using MMEGateWayCSharp.Core;
using MMEGateWayCSharp.Models;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MMEMessages.Models;

namespace MMESampleServer
{
    class ServerProgram
    {
        private const int Port = 12345; // Use the same port as your client
        private static bool _running = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");

            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            Console.WriteLine($"Server listening on port {Port}");

            while (_running)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected.");

                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }

                Thread.Sleep(100);
            }

            listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            try
            {
                ByteBuffer headerBuffer = ByteBuffer.Allocate(3);
                ByteBuffer dataBuffer = ByteBuffer.Allocate(1024);

                while (client.Connected)
                {
                    // Read header
                    if (ReadFromStream(stream, headerBuffer))
                    {
                        headerBuffer.Flip();
                        SoupHeader header = new SoupHeader();
                        header.Read(headerBuffer);
                        headerBuffer.Clear();

                        // Read data
                        dataBuffer.Limit = header.PayloadLength;
                        if (ReadFromStream(stream, dataBuffer))
                        {
                            dataBuffer.Flip();

                            // Process message
                            ProcessMessage(header, dataBuffer, stream);

                            dataBuffer.Clear();
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                stream.Close();
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }

        private static bool ReadFromStream(NetworkStream stream, ByteBuffer buffer)
        {
            while (buffer.HasRemaining)
            {
                byte[] tempBuffer = new byte[buffer.Remaining];
                int bytesRead = stream.Read(tempBuffer, 0, tempBuffer.Length);
                if (bytesRead == 0)
                {
                    return false;
                }
                buffer.Put(tempBuffer, 0, bytesRead);
            }
            return true;
        }

        private static void ProcessMessage(SoupHeader header, ByteBuffer dataBuffer, NetworkStream stream)
        {
            switch (header.Type)
            {
                case SoupConstants.TYPE_LOGIN:
                    // Handle login (for simplicity, always accept)
                    Console.WriteLine("Received login message.");
                    SendAcceptMessage(stream);
                    break;
                case SoupConstants.TYPE_UNSEQ:
                    // Handle unsequenced messages
                    ProcessUnsequencedMessage(dataBuffer, stream);
                    break;
                default:
                    Console.WriteLine($"Received unknown message type: {header.Type}");
                    break;
            }
        }

        private static void SendAcceptMessage(NetworkStream stream)
        {
            ByteBuffer headerBuffer = ByteBuffer.Allocate(3);
            ByteBuffer dataBuffer = ByteBuffer.Allocate(0);

            SoupHeader header = new SoupHeader
            {
                Length = 1,
                Type = SoupConstants.TYPE_ACCEPT
            };
            header.Write(headerBuffer);
            headerBuffer.Flip();

            stream.Write(headerBuffer.ToArray(), 0, headerBuffer.Limit);
            Console.WriteLine("Sent accept message.");
        }

        private static void ProcessUnsequencedMessage(ByteBuffer dataBuffer, NetworkStream stream)
        {
            // Read the message group and message ID
            long clientSequenceNumber = dataBuffer.GetLong();
            short messageGroup = dataBuffer.GetShort();
            short messageId = dataBuffer.GetShort();

            Console.WriteLine($"Received message: Group={messageGroup}, ID={messageId}, ClientSeqNum={clientSequenceNumber}");

            // For simplicity, assume success and send a CommitResponseMessage
            ServerCommitResponseMessage response = new ServerCommitResponseMessage
            {
                PartitionId = 0,
                StatusCode = 0, // 0 for success
                ClientSequenceNumber = clientSequenceNumber
            };

            // Send response
            ByteBuffer headerBuffer = ByteBuffer.Allocate(3);
            ByteBuffer responseDataBuffer = ByteBuffer.Allocate(15); // Adjust size as needed

            // Write response data
            response.Write(responseDataBuffer);
            responseDataBuffer.Flip();

            // Write header
            SoupHeader header = new SoupHeader
            {
                Length = responseDataBuffer.Limit + 1,
                Type = SoupConstants.TYPE_UNSEQ
            };
            header.Write(headerBuffer);
            headerBuffer.Flip();

            // Send header and data
            stream.Write(headerBuffer.ToArray(), 0, headerBuffer.Limit);
            stream.Write(responseDataBuffer.ToArray(), 0, responseDataBuffer.Limit);

            Console.WriteLine("Sent CommitResponseMessage.");
        }
    }
}
