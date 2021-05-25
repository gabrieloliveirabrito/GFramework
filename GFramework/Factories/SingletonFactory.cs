using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Factories
{
    using Interfaces;
    using Bases;

    public class SingletonFactory : BaseFactory<Type, ISingleton>
    {
        private static object syncLock = new object();

        private static SingletonFactory _instance;
        protected static SingletonFactory instance
            => _instance ?? (_instance = new SingletonFactory());

        public static TSingleton RegisterSingleton<TSingleton>()
            where TSingleton : class, ISingleton
        {
            return (TSingleton)RegisterSingleton(typeof(TSingleton));
        }

        public static ISingleton RegisterSingleton(Type singletonType)
        {
            lock (syncLock)
            {
                ISingleton singleton;
                if (!typeof(ISingleton).IsAssignableFrom(singletonType) || singletonType.IsInterface || singletonType.IsAbstract)
                    throw new InvalidOperationException("Invalid singleton type!");
                else if (instance.TryGetInstance(singletonType, out singleton))
                    return singleton;
                else
                {
                    singleton = Activator.CreateInstance(singletonType) as ISingleton;

                    if (instance.TryRegisterInstance(singletonType, singleton))
                    {
                        singleton.Created();

                        return singleton;
                    }
                    else
                        throw new InvalidOperationException("Failed to register a singleton instance!");
                }
            }
        }

        public static TSingleton DestroySingleton<TSingleton>() 
            where TSingleton : class, ISingleton
        {
            var singleton = DestroySingleton(typeof(TSingleton));
            return singleton == null ? null : (TSingleton)singleton;
        }

        public static ISingleton DestroySingleton(Type singletonType)
        {
            ISingleton singleton = instance.RemoveInstance(singletonType);
            singleton.Destroyed();

            return singleton;
        }

        public static void DestroyAll()
        {
            instance.RemoveAllInstances((k, v) => v.Destroyed());
        }
    }
}
