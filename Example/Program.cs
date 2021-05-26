using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using GFramework.Factories;
using GFramework.LogWriters;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerFactory.AddLogWriter<FileLogWriter>();

            var log = LoggerFactory.GetLogger<Program>();
            log.LogInfo("Info message");
            log.LogSuccess("Success message");
            log.LogWarning("Warning message");
            log.LogError("Error message");
            log.LogDebug("Debug message");
            log.LogFatal(new Exception("Fatal message"));

            bool serve;
            while (true)
            {
                log.LogInfo("Enter S to server, or C for client: ");
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
                    log.LogWarning("Invalid operation!");
                }
            }

            if (serve)
            {
                ChatServer server = new ChatServer();

                log.LogWarning("Enter without string to exit, with string to send message");

                string line;
                while ((line = Console.ReadLine()) != "")
                    server.SendMessage(line);
            }
            else
            {
                ChatClient client = new ChatClient();

                log.LogWarning("Enter without string to exit, with string to send message");

                string line;
                while ((line = Console.ReadLine()) != "")
                    if (line.ToLowerInvariant() == "ping")
                        client.Ping();
                    else
                        client.SendMessage(line);
            }

            log.LogWarning("Press Enter to exit...");
            Console.ReadLine();
            SingletonFactory.DestroyAll();
        }
    }
}