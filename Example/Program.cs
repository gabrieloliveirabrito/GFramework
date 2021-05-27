using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Example.PacketWriters;
using GFramework.Factories;
using GFramework.LogWriters;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerFactory.AddLogWriter<FileLogWriter>();

            var logger = LoggerFactory.GetLogger<Program>();
            logger.LogInfo("Info message");
            logger.LogSuccess("Success message");
            logger.LogWarning("Warning message");
            logger.LogError("Error message");
            logger.LogDebug("Debug message");
            logger.LogFatal(new Exception("Fatal message"));

            bool serve;
            while (true)
            {
                logger.LogInfo("Enter S to server, or C for client: ");
                var line = Console.ReadLine().ToLowerInvariant();

                if(line == "c")
                {
                    serve = false;
                    break;
                }
                else if(line == "s")
                {
                    serve = true;
                    break;
                }
                else
                {
                    logger.LogWarning("Invalid operation!");
                }
            }

            if (serve)
            {
                ChatServerWrapper server = new ChatServerWrapper();
                /*server.Server.OnServerOpened += (s, e) =>
                {
                    ChatClient[] clients = new ChatClient[server.Server.MaximumClients - 1];
                    for(int i = 0, n = clients.Length; i < n; i++)
                    {
                        var client = clients[i] = new ChatClient();
                        client.Connect();
                        //client.SendMessage($"Cliente {i}/{n}");

                        Thread.Sleep(100);
                    }
                };*/
                server.Open();

                logger.LogWarning("Enter without string to exit, with string to send message");

                string line;
                while ((line = Console.ReadLine()) != "")
                    server.SendToAll(new ChatMessageWriter { Message = line });
            }
            else
            {
                ChatClientWrapper client = new ChatClientWrapper();
                if (client.Connect())
                    logger.LogSuccess("Connect request has been sent! Response on event!");
                else
                    logger.LogError("Failed to sent connect request!");

                logger.LogWarning("Enter without string to exit, with string to send message");

                string line;
                while ((line = Console.ReadLine()) != "")
                    if (line.ToLowerInvariant() == "ping")
                        client.Socket.Ping();
                    else
                        client.Send(new ChatMessageWriter { Message = line });
            }

            logger.LogWarning("Press Enter to exit...");
            Console.ReadLine();
            SingletonFactory.DestroyAll();
        }
    }
}