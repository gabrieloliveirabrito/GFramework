using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Factories
{
    using Bases;
    using Holders;
    using Interfaces;

    public class ComponentFactory : BaseFactory<Type, ComponentHolder>, ISingleton
    {
        private static ComponentFactory Instance => SingletonFactory.RegisterSingleton<ComponentFactory>();
        private static BaseLogger log;

        void ISingleton.Created()
        {
            log = LoggerFactory.GetLogger<ComponentFactory>();
            log.LogInfo("ComponentFactory has been enabled!");
        }

        void ISingleton.Destroyed()
        {
            Instance.RemoveAllInstances((t, holder) => holder.Disable());
        }

        public static bool Enable<TComponent>()
            where TComponent : IComponent
        {
            return Enable(typeof(TComponent));
        }

        public static bool Enable(Type componentType)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
                throw new InvalidCastException("The component not implements the IComponent interface!");

            if (Instance.TryGetInstance(componentType, out ComponentHolder holder))
                return holder.Enable();
            else
                return Enable(Activator.CreateInstance(componentType) as IComponent);
        }

        public static bool Enable(IComponent component)
        {
            var type = component.GetType();
            if (Instance.TryGetInstance(type, out ComponentHolder holder))
                return holder.Enable();
            else
            {
                holder = Instance.RegisterInstance(type, new ComponentHolder(component));
                return holder.Enable();
            }
        }
        public static bool Disable<TComponent>()
           where TComponent : IComponent
        {
            return Disable(typeof(TComponent));
        }

        public static bool Disable(Type componentType)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
                throw new InvalidCastException("The component not implements the IComponent interface!");

            if (Instance.TryGetInstance(componentType, out ComponentHolder holder))
                return holder.Disable();
            else
                return false;
        }

        public static bool Disable(IComponent component)
        {
            var type = component.GetType();
            if (Instance.TryGetInstance(type, out ComponentHolder holder))
                return holder.Disable();
            else
                return false;
        }
    }
}
