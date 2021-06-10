using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.EventArgs
{
    using Enums;

    public class LogEventArgs : System.EventArgs
    {
        public DateTime Time { get; private set; }
        public LogType Type { get; private set; }
        public string Name { get; private set; }
        public string Message { get; private set; }

        public LogEventArgs(DateTime time, LogType type, string name, string message)
        {
            Time = time;
            Type = type;
            Name = name;
            Message = message;
        }
    }
}
