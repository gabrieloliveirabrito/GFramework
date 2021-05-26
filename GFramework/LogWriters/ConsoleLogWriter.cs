using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.LogWriters
{
    using Bases;
    using Enums;

    public class ConsoleLogWriter : BaseLogWriter
    {
        public override void Write(BaseLog log)
        {
            Console.ResetColor();
            Console.Write("{0} - ", log.Time);

            switch(log.Type)
            {
                case LogType.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogType.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
            Console.Write(log.Type.ToString().ToUpperInvariant());

            Console.ResetColor();
            Console.WriteLine(" - {0}: {1}", log.Name, log.Message);

        }
    }
}
