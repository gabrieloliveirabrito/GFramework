using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Bases
{
    using Factories;

    public abstract class BaseLogger
    {
        public string Name { get; internal set; }

        protected void AppendLog(Action callback)
        {
            LoggerFactory.AppendLog(callback);
        }

        public abstract void LogDebug(object message);
        public abstract void LogDebug(string message, params object[] args);
        public abstract void LogError(object message);
        public abstract void LogError(string message, params object[] args);
        public abstract void LogFatal(Exception ex);
        public abstract void LogInfo(object message);
        public abstract void LogInfo(string message, params object[] args);
        public abstract void LogSuccess(object message);
        public abstract void LogSuccess(string message, params object[] args);
        public abstract void LogWarning(object message);
        public abstract void LogWarning(string message, params object[] args);
    }
}
