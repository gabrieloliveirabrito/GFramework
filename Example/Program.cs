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

            if (Console.ReadKey().Key == ConsoleKey.S)
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
                    client.SendMessage(line);
            }

            log.LogWarning("Press Enter to exit...");
            Console.ReadLine();
            SingletonFactory.DestroyAll();
        }
    }
}