using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Bases
{
    public abstract class BaseFactory<TKey, TInstance>
        where TInstance : class
    {
        private static object syncLock = new object();
        private volatile Dictionary<TKey, TInstance> pairs;

        public BaseFactory()
        {
            pairs = new Dictionary<TKey, TInstance>();
        }

        protected TInstance RegisterInstance(TKey key, TInstance instance)
        {
            lock (syncLock)
            {
                if (pairs.ContainsKey(key))
                    throw new InvalidOperationException("Cannot add a instance that already registered!");

                return pairs[key] = instance;
            }
        }

        protected bool TryRegisterInstance(TKey key, TInstance instance)
        {
            RegisterInstance(key, instance);
            return true;
        }

        protected bool TryGetInstance(TKey key, out TInstance instance)
        {
            lock (syncLock)
            {
                return pairs.TryGetValue(key, out instance);
            }
        }

        protected bool TryGetInstanceByType<T>(out TInstance instance)
            where T : TKey
        {
            lock (syncLock)
            {
                var pair = pairs.FirstOrDefault(p => p.Key.GetType() == typeof(T));
                instance = pair.Equals(default(KeyValuePair<TKey, TInstance>)) ? null : pair.Value;

                return instance != null;
            }
        }

        protected TInstance RemoveInstance(TKey key)
        {
            lock (syncLock)
            {
                if (!pairs.ContainsKey(key))
                    throw new InvalidOperationException("Cannot remove a instance that not registered!");

                TInstance instance = pairs[key];
                pairs.Remove(key);

                return instance;
            }
        }

        protected bool TryRemoveInstance(TKey key, out TInstance instance)
        {
            try
            {
                instance = RemoveInstance(key);
                return true;
            }
            catch (Exception)
            {
                instance = null;
                return false;
            }
        }

        protected void RemoveAllInstances(Action<TKey, TInstance> callback = null)
        {
            lock (syncLock)
            {
                if (callback != null)
                    foreach (var pair in pairs)
                        callback(pair.Key, pair.Value);

                pairs.Clear();
            }
        }
    }
}
