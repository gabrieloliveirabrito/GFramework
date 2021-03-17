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
        private static QueueFactory Instance => SingletonFactory.RegisterSingleton<QueueFactory>();

        void ISingleton.Created()
        {
            
        }

        void ISingleton.Destroyed()
        {
            
        }

        public static bool IsEmpty(IQueue queue)
        {
            if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                if (!Instance.TryRegisterInstance(queue, nativeQueue = new Queue<object>()))
                    throw new InvalidOperationException("The queue hasn't registered");

            return nativeQueue.Count() == 0;
        }

        public static void ClearQueue(IQueue queue)
        {
            if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                if (!Instance.TryRegisterInstance(queue, nativeQueue = new Queue<object>()))
                    throw new InvalidOperationException("The queue hasn't registered");

            nativeQueue.Clear();
        }

        public static void Enqueue(IQueue queue, object value)
        {
            if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                if (!Instance.TryRegisterInstance(queue, nativeQueue = new Queue<object>()))
                    throw new InvalidOperationException("The queue hasn't registered");

            nativeQueue.Enqueue(value);
        }

        public static T Dequeue<T>(IQueue queue)
            => (T)Dequeue(queue);

        public static object Dequeue(IQueue queue)
        {
            if (!Instance.TryGetInstance(queue, out Queue<object> nativeQueue))
                throw new InvalidOperationException("The queue hasn't registered");
            else
                return nativeQueue.Dequeue();
        }
    }
}