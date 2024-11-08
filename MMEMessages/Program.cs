﻿// See https://aka.ms/new-console-template for more information
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
            .AddScoped<IByteMessage, SoupLogin>() // Assuming SoupClient implements ISoupClient
            .AddScoped<IByteMessage, SoupReject>()
            .AddScoped<IConnection, SoupConnection>()
            .AddTransient<App>() // Register the main app class
            .BuildServiceProvider();

        // Resolve and run the main app
        var app = serviceProvider.GetRequiredService<App>();
        await app.RunAsync();
    }
}






public class App
{
    public async Task RunAsync()
    {
        string serverAddress = "127.0.0.1";
        int serverPort = 19100;
        string userName = "DRPORD";
        string password = "4lso6iY?bl";

        try
        {
            // Create the TaskCompletionSource for login
            var loginCompletionSource = new TaskCompletionSource<bool>();

            // Create a SoupClient
            var soupClient = SoupClient.Create();

            // Create a ConnectionListener with credentials and the loginCompletionSource
            var listener = new ConnectionListener(userName, password, loginCompletionSource);

            // Create a connection
            var connection = soupClient.CreateConnection(serverAddress, serverPort, listener);

            // Optionally set tracing
            connection.SetTrace(true);

            // **Initiate Login Immediately After Connection**
            connection.Login(userName, password);

            // **Await the login process**
            await loginCompletionSource.Task;

            if (connection.IsLoggedIn)
            {
                Console.WriteLine("Successfully logged in.");

                // Continue with sending messages...

                // Wait to receive messages or process further
                await Task.Delay(5000);
            }
            else
            {
                Console.WriteLine("Failed to log in.");
            }

            // Logout and close the connection
            connection.Logout();
            soupClient.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
