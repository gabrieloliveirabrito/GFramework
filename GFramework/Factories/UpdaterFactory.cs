﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Factories
{
    using Bases;
    using Enums;
    using Interfaces;

    public class UpdaterFactory : BaseFactory<IUpdater, BaseUpdater>, ISingleton
    {
        private static UpdaterFactory Instance => SingletonFactory.RegisterSingleton<UpdaterFactory>();

        public static void Start(IUpdater updater)
        {
            if (!Instance.TryGetInstance(updater, out BaseUpdater baseUpdater))
                if (!Instance.TryRegisterInstance(updater, baseUpdater = new BaseUpdater(updater)))
                    throw new InvalidOperationException("The queue hasn't registered");

            baseUpdater.Start();
        }

        public static void Stop(IUpdater updater)
        {
            if (!Instance.TryGetInstance(updater, out BaseUpdater baseUpdater))
                throw new InvalidOperationException("The updater has not registered!");
            else if(baseUpdater.IsRunning)
                baseUpdater.Stop();
        }

        public static bool IsRunning(IUpdater updater)
        {
            return Instance.TryGetInstance(updater, out BaseUpdater baseUpdater) ? baseUpdater.IsRunning : false;
        }

        public static bool IsRunning<TUpdater>()
            where TUpdater : IUpdater
        {
            return Instance.TryGetInstanceByType<TUpdater>(out BaseUpdater baseUpdater) ? baseUpdater.IsRunning : false;
        }

        void ISingleton.Created()
        {
          
        }

        void ISingleton.Destroyed()
        {
            Instance.RemoveAllInstances((u, baseUpdater) => baseUpdater.Stop());
        }
    }
}
