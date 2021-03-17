using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFramework.Factories;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = LoggerFactory.GetLogger<Program>();
            log.LogInfo("Info message");
            log.LogSuccess("Success message");
            log.LogWarning("Warning message");
            log.LogError("Error message");
            log.LogDebug("Debug message");
            log.LogFatal(new Exception("Fatal message"));

            Task.Run(async () =>
            {
                while (true)
                {
                    log.LogWarning("Press Enter to exit...");
                    await Task.Delay(1000);
                }
            });

            Console.ReadLine();
        }
    }
}