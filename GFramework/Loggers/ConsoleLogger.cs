using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Loggers
{
    using Bases;

    public class ConsoleLogger : BaseLogger
    {
        void AppendLog(ConsoleColor color, string type, object message)
        {
            var date = DateTime.Now;
            AppendLog(new Action(() =>
            {
                Console.ResetColor();
                Console.Write("{0} - ", date);

                Console.ForegroundColor = color;
                Console.Write(type);

                Console.ResetColor();
                Console.WriteLine(" - {0}: {1}", Name, message);
            }));
        }

        public override void LogDebug(object message)
        {
            AppendLog(ConsoleColor.Gray, "DEBUG", message);
        }

        public override void LogDebug(string message, params object[] args)
        {
            AppendLog(ConsoleColor.Gray, "DEBUG", string.Format(message, args));
        }

        public override void LogError(object message)
        {
            AppendLog(ConsoleColor.Red, "ERROR", message);
        }

        public override void LogError(string message, params object[] args)
        {
            AppendLog(ConsoleColor.Red, "ERROR", string.Format(message, args));
        }

        public override void LogFatal(Exception ex)
        {
            AppendLog(ConsoleColor.DarkRed, "FATAL", ex);
        }

        public override void LogInfo(object message)
        {
            AppendLog(ConsoleColor.Cyan, "INFO", message);
        }

        public override void LogInfo(string message, params object[] args)
        {
            AppendLog(ConsoleColor.Cyan, "INFO", string.Format(message, args));
        }

        public override void LogSuccess(object message)
        {
            AppendLog(ConsoleColor.Green, "SUCCESS", message);
        }

        public override void LogSuccess(string message, params object[] args)
        {
            AppendLog(ConsoleColor.Green, "SUCCESS", string.Format(message, args));
        }

        public override void LogWarning(object message)
        {
            AppendLog(ConsoleColor.Yellow, "WARNING", message);
        }

        public override void LogWarning(string message, params object[] args)
        {
            AppendLog(ConsoleColor.Yellow, "WARNING", string.Format(message, args));
        }
    }
}
