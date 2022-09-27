using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Factories
{
    using Bases;
    using Enums;
    using Holders;
    using Interfaces;
    using LogWriters;

    public class LoggerFactory : BaseFactory<string, BaseLogger>, ISingleton, IUpdater, IQueue
    {
        private static LoggerFactory Instance => SingletonFactory.RegisterSingleton<LoggerFactory>();

        private List<BaseLogWriter> writers;

        private static Type loggerType = typeof(BaseLogger);
        public static Type DefaultLoggerType
        {
            get => loggerType;
            set
            {
                if (!value.IsSubclassOf(typeof(BaseLogger)))
                    throw new InvalidCastException("A Logger type need to implements the BaseLogger interface!");

                loggerType = value;
            }
        }

        uint IUpdater.Interval => QueueFactory.IsEmpty(this) ? 100u : 1u;
        UpdaterMode IUpdater.Mode => UpdaterMode.DelayAfter;

        void ISingleton.Created()
        {
            writers = new List<BaseLogWriter>();
            AddLogWriter<ConsoleLogWriter>();

            QueueFactory.RegisterQueue(this);
            UpdaterFactory.Start(this);
        }

        void ISingleton.Destroyed()
        {
            if (UpdaterFactory.IsRunning(this))
                UpdaterFactory.Stop(this);
        }

        Task IUpdater.Started()
        {
            return Task.CompletedTask;
        }

        Task IUpdater.Run()
        {
            if (!QueueFactory.IsEmpty(this))
            {
                LogHolder log = QueueFactory.Dequeue<LogHolder>(this);

                foreach (BaseLogWriter writer in writers)
                    writer.Write(log);
            }
            return Task.CompletedTask;
        }

        Task IUpdater.Stopped()
        {
            Action[] callbacks = QueueFactory.DequeueAll<Action>(this);

            foreach (Action callback in callbacks)
                callback();

            return Task.CompletedTask;
        }

        BaseLogger InitializeLogger(Type loggerType, string name)
        {
            if (loggerType == null)
                throw new NullReferenceException("LoggerType can't be null!");
            else if (loggerType != typeof(BaseLogger) && !loggerType.IsSubclassOf(typeof(BaseLogger)))
                throw new InvalidCastException("A Logger type need to implements the BaseLogger interface!");

            BaseLogger logger = (BaseLogger)Activator.CreateInstance(loggerType, name);
            logger.Name = name;
            logger.Factory = this;

            return logger;
        }

        public static void ClearLogWriters()
        {
            Instance.writers.Clear();
        }

        public static void AddLogWriter<TLogWriter>()
            where TLogWriter : BaseLogWriter
        {
            if (!Instance.writers.Any(w => w.GetType() == typeof(TLogWriter)))
                Instance.writers.Add(Activator.CreateInstance<TLogWriter>());
        }

        public static BaseLogger GetLogger(string name)
        {
            if (Instance.TryGetInstance(name, out BaseLogger logger))
                return logger;
            else
                return Instance.RegisterInstance(name, Instance.InitializeLogger(DefaultLoggerType, name));
        }

        public static BaseLogger GetLogger<T>() => GetLogger(typeof(T));
        public static BaseLogger GetLogger(Type type) => GetLogger(type.GetRealTypeName());
        public static BaseLogger GetLogger<T>(T _) => GetLogger(typeof(T));

        protected internal void AppendLog(LogType type, string name, string message)
        {
            if (UpdaterFactory.IsRunning(this))
                QueueFactory.Enqueue(this, new LogHolder(type, name, message));
        }
    }
}