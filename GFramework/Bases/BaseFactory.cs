﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Bases
{
    public abstract class BaseFactory<TKey, TInstance>
        where TInstance : class
    {
        private Dictionary<TKey, TInstance> pairs;

        public BaseFactory()
        {
            pairs = new Dictionary<TKey, TInstance>();
        }

        protected TInstance RegisterInstance(TKey key, TInstance instance)
        {
            if (pairs.ContainsKey(key))
                throw new InvalidOperationException("Cannot add a instance that already registered!");

            return pairs[key] = instance;
        }

        protected bool TryRegisterInstance(TKey key, TInstance instance)
        {
            try
            {
                RegisterInstance(key, instance);
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log
                return false;
            }
        }

        protected bool TryGetInstance(TKey key, out TInstance instance)
        {
            return pairs.TryGetValue(key, out instance);
        }

        protected TInstance RemoveInstance(TKey key)
        {
            if (!pairs.ContainsKey(key))
                throw new InvalidOperationException("Cannot remove a instance that not registered!");

            TInstance instance = pairs[key];
            pairs.Remove(key);

            return instance;
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
            if (callback != null)
                foreach (var pair in pairs)
                    callback(pair.Key, pair.Value);

            pairs.Clear();
        }
    }
}
