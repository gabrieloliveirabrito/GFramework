using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Factories
{
    using Bases;
    using EventArgs;
    using Interfaces;

    public class SingletonFactory : BaseFactory<Type, ISingleton>
    {
        private static SingletonFactory _instance;
        protected static SingletonFactory instance
            => _instance ?? (_instance = new SingletonFactory());

        public static event EventHandler<SingletonEventArgs> OnSingletonCreated, OnSingletonDestroyed;

        public static TSingleton RegisterSingleton<TSingleton>()
            where TSingleton : class, ISingleton
        {
            return (TSingleton)RegisterSingleton(typeof(TSingleton));
        }

        public static ISingleton RegisterSingleton(Type singletonType)
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
                    if (OnSingletonCreated != null)
                        OnSingletonCreated(instance, new SingletonEventArgs(singleton));
                    return singleton;
                }
                else
                    throw new InvalidOperationException("Failed to register a singleton instance!");
            }
        }

        public static void RegisterSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : ISingleton
        {
            if (instance.TryRegisterInstance(typeof(TSingleton), singleton))
            {
                singleton.Created();
                if (OnSingletonCreated != null)
                    OnSingletonCreated(instance, new SingletonEventArgs(singleton));
            }
            else
                throw new InvalidOperationException("Failed to register a singleton instance!");
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
            if (OnSingletonDestroyed != null)
                OnSingletonDestroyed(instance, new SingletonEventArgs(singleton));
            return singleton;
        }

        public static void DestroyAll()
        {
            instance.RemoveAllInstances((k, v) =>
            {
                v.Destroyed();
                if (OnSingletonDestroyed != null)
                    OnSingletonDestroyed(instance, new SingletonEventArgs(v));
            });
        }
    }
}
