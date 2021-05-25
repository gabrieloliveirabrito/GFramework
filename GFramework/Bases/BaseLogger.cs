using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Bases
{
    using Enums;
    using Factories;

    public class BaseLogger
    {
        internal virtual LoggerFactory Factory { get; set; }
        public string Name { get; internal set; }

        public BaseLogger(string name)
        {
            Name = name;
        }

        string CreateExceptionMessage(Exception ex)
        {
            if (ex == null)
                return "NULL";
            else
                return ex.GetType().Name + " - " + ex.Message + Environment.NewLine + ex.StackTrace;
        }

        public virtual void LogDebug(object message) => Factory.AppendLog(LogType.Debug, Name, (message ?? "NULL").ToString());
        public virtual void LogDebug(string message, params object[] args) => Factory.AppendLog(LogType.Debug, Name, string.Format(message ?? "NULL", args));
        public virtual void LogError(object message) => Factory.AppendLog(LogType.Error, Name, (message ?? "NULL").ToString());
        public virtual void LogError(string message, params object[] args) => Factory.AppendLog(LogType.Error, Name, string.Format(message ?? "NULL", args));
        public virtual void LogFatal(Exception ex) => Factory.AppendLog(LogType.Fatal, Name, CreateExceptionMessage(ex));
        public virtual void LogInfo(object message) => Factory.AppendLog(LogType.Info, Name, (message ?? "NULL").ToString());
        public virtual void LogInfo(string message, params object[] args) => Factory.AppendLog(LogType.Info, Name, string.Format(message ?? "NULL", args));
        public virtual void LogSuccess(object message) => Factory.AppendLog(LogType.Success, Name, (message ?? "NULL").ToString());
        public virtual void LogSuccess(string message, params object[] args) => Factory.AppendLog(LogType.Success, Name, string.Format(message ?? "NULL", args));
        public virtual void LogWarning(object message) => Factory.AppendLog(LogType.Warning, Name, (message ?? "NULL").ToString());
        public virtual void LogWarning(string message, params object[] args) => Factory.AppendLog(LogType.Warning, Name, string.Format(message ?? "NULL", args));
    }
}
