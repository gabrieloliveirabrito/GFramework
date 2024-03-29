﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Holders
{
    using Enums;

    public class LogHolder
    {
        public DateTime Time { get; set; }
        public LogType Type { get; private set; }
        public string Name { get; private set; }
        public string Message { get; set; }

        public LogHolder(LogType type, string name, string message)
        {
            Time = DateTime.Now;
            Type = type;
            Name = name;
            Message = message;
        }
    }
}
