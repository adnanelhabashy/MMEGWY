using MMEGateWayCSharp.Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class MockSoupServer
{
    static async Task Main(string[] args)
    {
        int port = 19100;
        TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        listener.Start();
        Console.WriteLine($"Mock Soup Server listening on port {port}...");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");
            _ = HandleClientAsync(client);
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        using (NetworkStream stream = client.GetStream())
        {
            try
            {
                while (true)
                {
                    // Read Header (3 bytes)
                    byte[] header = new byte[3];
                    int bytesRead = await stream.ReadAsync(header, 0, 3);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    // Parse Header
                    int length = (header[0] << 8) | header[1]; // BigEndian
                    byte type = header[2];
                    Console.WriteLine($"Received Header: Length={length}, Type={(char)type}");

                    // Read Payload
                    byte[] payload = new byte[length - 1];
                    bytesRead = await stream.ReadAsync(payload, 0, payload.Length);
                    if (bytesRead < payload.Length)
                    {
                        Console.WriteLine("Incomplete payload received.");
                        break;
                    }

                    if (type == SoupConstants.TYPE_LOGIN)
                    {
                        string userName = Encoding.ASCII.GetString(payload, 0, SoupConstants.USER_NAME_LENGTH).Trim();
                        string password = Encoding.ASCII.GetString(payload, SoupConstants.USER_NAME_LENGTH, SoupConstants.PASSWORD_LENGTH).Trim();
                        string session = Encoding.ASCII.GetString(payload, SoupConstants.USER_NAME_LENGTH + SoupConstants.PASSWORD_LENGTH, SoupConstants.SESSION_LENGTH).Trim();
                        string sequenceNumberStr = Encoding.ASCII.GetString(payload, SoupConstants.USER_NAME_LENGTH + SoupConstants.PASSWORD_LENGTH + SoupConstants.SESSION_LENGTH, SoupConstants.SEQUENCE_NUMBER_LENGTH).Trim();
                        long sequenceNumber = long.Parse(sequenceNumberStr);

                        Console.WriteLine($"Login Attempt - UserName: {userName}, Password: {password}, Session: {session}, SequenceNumber: {sequenceNumber}");

                        // Simulate Accepting Login
                        await Task.Delay(100); // Simulate processing delay

                        // Prepare TYPE_ACCEPT message
                        string acceptSession = "XYZ123    "; // Padded to 10 characters
                        string acceptSequenceNumber = "0000000001"; // Padded to 20 characters
                        byte[] acceptPayload = Encoding.ASCII.GetBytes(acceptSession + acceptSequenceNumber);
                        int acceptLength = acceptPayload.Length + 1; // Type byte + payload

                        byte[] acceptHeader = new byte[3];
                        acceptHeader[0] = (byte)((acceptLength >> 8) & 0xFF); // BigEndian
                        acceptHeader[1] = (byte)(acceptLength & 0xFF);
                        acceptHeader[2] = SoupConstants.TYPE_ACCEPT; // 'A'

                        Console.WriteLine($"Sending Accept Header: {BitConverter.ToString(acceptHeader)}");
                        Console.WriteLine($"Sending Accept Payload: {Encoding.ASCII.GetString(acceptPayload)}");

                        // Send Header
                        await stream.WriteAsync(acceptHeader, 0, acceptHeader.Length);
                        // Send Payload
                        await stream.WriteAsync(acceptPayload, 0, acceptPayload.Length);
                    }
                    else if (type == SoupConstants.TYPE_LOGOUT)
                    {
                        Console.WriteLine("Received Logout message.");
                        break; // Close connection
                    }
                    else if (type == SoupConstants.TYPE_CLI_HEARTBEAT)
                    {
                        Console.WriteLine("Received Client Heartbeat.");

                        // Respond with Server Heartbeat
                        byte[] srvHeartbeatHeader = new byte[3];
                        srvHeartbeatHeader[0] = 0x00;
                        srvHeartbeatHeader[1] = 0x01;
                        srvHeartbeatHeader[2] = SoupConstants.TYPE_SRV_HEARTBEAT; // 'H'

                        Console.WriteLine($"Sending Server Heartbeat Header: {BitConverter.ToString(srvHeartbeatHeader)}");

                        await stream.WriteAsync(srvHeartbeatHeader, 0, srvHeartbeatHeader.Length);
                    }
                    else
                    {
                        Console.WriteLine($"Unknown message type received: {(char)type}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in client handler: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Client disconnected.");
            }
        }
    }
}
