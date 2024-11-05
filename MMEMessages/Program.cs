// See https://aka.ms/new-console-template for more information


using MMEGateWayCSharp.Core;
using MMEGateWayCSharp.Exceptions;
using MMEMessages.Listeners;
using MMEMessages.Models;

string serverAddress = "your.server.address";
int serverPort = 12345;

// Replace with your credentials
string userName = "yourUserName";
string password = "yourPassword";



Console.WriteLine("Hello, World!");



try
{
    // Create a SoupClient
    var soupClient = SoupClient.Create();

    // Create a ConnectionListener
    var listener = new ConnectionListener();

    // Create a connection
    var connection = soupClient.CreateConnection(serverAddress, serverPort, listener);

    // Optionally set tracing
    connection.SetTrace(true);

    // Login to the server
    connection.Login(userName, password);

    // Wait for the connection to be established
    // In a real application, you would handle this asynchronously
    Thread.Sleep(2000);

    if (connection.IsLoggedIn)
    {
        Console.WriteLine("Successfully logged in.");

        // Create a MmeCommit message
        var mmeCommit = new MmeCommit
        {
            MessageGroup = MmeCommit.COMMIT_MESSAGE_GROUP,
            MessageId = 1,
            PartitionId = 0,
            StatusCode = 200,
            ClientSequenceNumber = 1234567890L
        };

        // Send MmeCommit message
        connection.SendMessage(mmeCommit);
        Console.WriteLine("MmeCommit message sent.");

        // Create a MmeReferencePrice message
        var mmeReferencePrice = new MmeReferencePrice
        {
            ClientSequenceNumber = 1234567891L,
            MessageGroup = 30,
            MessageId = 2,
            PartitionId = 0,
            ActorId = 1001,
            OrderBookId = 2002,
            ReferencePriceSource = 1,
            ReferencePrice = 1500000L, // Example price in smallest currency unit
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Send MmeReferencePrice message
        connection.SendMessage(mmeReferencePrice);
        Console.WriteLine("MmeReferencePrice message sent.");

        // Wait to receive messages or process further
        Thread.Sleep(5000);
    }
    else
    {
        Console.WriteLine("Failed to log in.");
    }

    // Logout and close the connection
    connection.Logout();
    soupClient.Close();
}
catch (SoupException ex)
{
    Console.WriteLine($"SoupException: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
}

