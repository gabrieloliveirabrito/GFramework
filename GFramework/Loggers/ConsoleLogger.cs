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
        public override void LogDebug(object message)
        {
            throw new NotImplementedException();
        }

        public override void LogDebug(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override void LogError(object message)
        {
            throw new NotImplementedException();
        }

        public override void LogError(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override void LogFatal(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override void LogInfo(object message)
        {
            throw new NotImplementedException();
        }

        public override void LogInfo(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override void LogSuccess(object message)
        {
            throw new NotImplementedException();
        }

        public override void LogSuccess(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override void LogWarning(object message)
        {
            throw new NotImplementedException();
        }

        public override void LogWarning(string message, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
