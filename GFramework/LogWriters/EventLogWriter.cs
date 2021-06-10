using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.LogWriters
{
    using Bases;
    using EventArgs;
    using Holders;

    public class EventLogWriter : BaseLogWriter
    {
        public static event EventHandler<LogEventArgs> OnLogWrite;

        public override void Write(LogHolder log)
        {
            if (OnLogWrite != null)
                OnLogWrite(this, new LogEventArgs(log.Time, log.Type, log.Name, log.Message));
        }
    }
}
