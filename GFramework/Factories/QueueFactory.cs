using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Factories
{
    using Bases;
    using Interfaces;

    public class QueueFactory : BaseFactory<IQueue, Queue<object>>, ISingleton
    {
        private static object syncLock = new object();
        private static QueueFactory Instance => SingletonFactory.RegisterSingleton<QueueFactory>();

        void ISingleton.Created()
        {
        }

        void ISingleton.Destroyed()
        {
            Instance.RemoveAllInstances((q, nativeQueue) => nativeQueue.Clear());
        }

        public static void RegisterQueue(IQueue queue)
        {
            if (!Instance.TryGetInstance(queue, out _))
                Instance.TryRegisterInstance(queue, new Queue<object>());
        }

        public static bool IsEmpty(IQueue queue)
        {
            lock (syncLock)
            {
                if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                    throw new InvalidOperationException("The queue hasn't registered");

                return nativeQueue.Count() == 0;
            }
        }

        public static void ClearQueue(IQueue queue)
        {
            lock (syncLock)
            {
                if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                    throw new InvalidOperationException("The queue hasn't registered");

                nativeQueue.Clear();
            }
        }

        public static void Enqueue(IQueue queue, object value)
        {
            lock (syncLock)
            {
                if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                    throw new InvalidOperationException("The queue hasn't registered");

                nativeQueue.Enqueue(value);
            }
        }

        public static T Dequeue<T>(IQueue queue)
            => (T)Dequeue(queue);

        public static object Dequeue(IQueue queue)
        {
            lock (syncLock)
            {
                if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                    throw new InvalidOperationException("The queue hasn't registered");
                else
                    return nativeQueue.Dequeue();
            }
        }

        public static T[] DequeueAll<T>(IQueue queue)
            => DequeueAll(queue).Cast<T>().ToArray();

        public static object[] DequeueAll(IQueue queue)
        {
            lock (syncLock)
            {
                if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                    throw new InvalidOperationException("The queue hasn't registered");
                else
                {
                    object[] items = nativeQueue.ToArray();
                    nativeQueue.Clear();

                    return items;
                }
            }
        }
    }
}