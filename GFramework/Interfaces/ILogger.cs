using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Interfaces
{
    public interface ILogger
    {
        string Name { get; }

        void LogInfo(object message);
        void LogInfo(string message, params object[] args);
        
        void LogDebug(object message);
        void LogDebug(string message, params object[] args);

        void LogWarning(object message);
        void LogWarning(string message, params object[] args);

        void LogError(object message);
        void LogError(string message, params object[] args);

        void LogSuccess(object message);
        void LogSuccess(string message, params object[] args);

        void LogFatal(Exception ex);
    }
}
