// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using MMEGateWayCSharp.Core;
using MMEGateWayCSharp.Exceptions;
using MMEGateWayCSharp.Interfaces;
using MMEGateWayCSharp.Models;
using MMEMessages.Listeners;
using MMEMessages.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Set up DI container
        var serviceProvider = new ServiceCollection()
            .AddScoped<IByteMessage, SoupAccept>()
            .AddScoped<IByteMessage, SoupHeader>()
            .AddScoped<IByteMessage, SoupLogin>()// Assuming SoupClient implements ISoupClient
            .AddScoped<IByteMessage, SoupReject>()
            .AddScoped<IConnection, SoupConnection>()
            .AddScoped<IConnectionListener>(provider =>new ConnectionListener("DRPORD", "4lso6iY?bl"))
            .AddTransient<App>() // Register the main app class
            .BuildServiceProvider();

        // Resolve and run the main app
        var app = serviceProvider.GetRequiredService<App>();
        await app.RunAsync();
    }
}




public class App
{
    private readonly SoupClient _soupClient = new();
    private readonly IConnectionListener _listener;

    public App(IConnectionListener listener)
    {
        _listener = listener;
    }

    public async Task RunAsync()
    {
        string serverAddress = "127.0.0.1";
        int serverPort = 19100;
        string userName = "DRPORD";
        string password = "4lso6iY?bl";

        try
        {
            // Create a SoupClient
            var soupClient = SoupClient.Create();

            // Create a ConnectionListener with credentials
            var listener = new ConnectionListener(userName, password);

            // Create a connection
            var connection = soupClient.CreateConnection(serverAddress, serverPort, listener);

            // Optionally set tracing
            connection.SetTrace(true);

            // The Login call is now handled within the OnConnectionEstablished method
            // So you don't need to call connection.Login() here

            // Wait for the connection to be fully established and logged in
            // You might want to implement a mechanism to wait for login confirmation
            // For simplicity, we'll wait for a few seconds here
            Thread.Sleep(5000);

            if (connection.IsLoggedIn)
            {
                Console.WriteLine("Successfully logged in.");

                // Continue with sending messages...

                // Create and send MmeCommit message
                var mmeCommit = new MmeCommit
                {
                    MessageGroup = MmeCommit.COMMIT_MESSAGE_GROUP,
                    MessageId = 1,
                    PartitionId = 0,
                    StatusCode = 200,
                    ClientSequenceNumber = 1234567890L
                };
                connection.SendMessage(mmeCommit);
                Console.WriteLine("MmeCommit message sent.");

                // Create and send MmeReferencePrice message
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
    }
}
