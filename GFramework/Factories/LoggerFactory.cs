using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Factories
{
    using Bases;
    using Interfaces;

    public class LoggerFactory : BaseFactory<string, BaseLogger>, ISingleton
    {
        private static LoggerFactory Instance => SingletonFactory.RegisterSingleton<LoggerFactory>();

        private static Type loggerType;
        public static Type DefaultLoggerType
        {
            get => loggerType;
            set
            {
                if (value.BaseType != typeof(BaseLogger)
                    throw new InvalidCastException("A Logger type need to implements the BaseLogger interface!");

                loggerType = value;
            }
        }

        void ISingleton.Created()
        {
        }

        void ISingleton.Destroyed()
        {
        }

        static BaseLogger InitializeLogger(Type loggerType, string name)
        {
            if (loggerType == null)
                throw new NullReferenceException("LoggerType can't be null!");
            else if (loggerType.BaseType != typeof(BaseLogger))
                throw new InvalidCastException("A Logger type need to implements the BaseLogger interface!");

            BaseLogger logger = (BaseLogger)Activator.CreateInstance(loggerType);
            logger.Name = name;

            return logger;
        }

        public static BaseLogger GetLogger(string name)
        {
            if (Instance.TryGetInstance(name, out BaseLogger logger))
                return logger;
            else
                return Instance.RegisterInstance(name, InitializeLogger(DefaultLoggerType, name));
        }

        public static TLogger GetLogger<TLogger>(string name)
            where TLogger : BaseLogger, new()
        {
            if (Instance.TryGetInstance(name, out BaseLogger logger))
                return (TLogger)logger;
            else
                return (TLogger)Instance.RegisterInstance(name, InitializeLogger(typeof(TLogger), name));
        }

        public static BaseLogger GetLogger<T>() => GetLogger(typeof(T));
        public static TLogger GetLogger<T, TLogger>() where TLogger : BaseLogger, new()
            => GetLogger<TLogger>(typeof(T));

        public static BaseLogger GetLogger(Type type) => GetLogger(type.Name);
        public static TLogger GetLogger<TLogger>(Type type) where TLogger : BaseLogger, new()
            => GetLogger<TLogger>(type.Name);
    }
}