using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Holders
{
    using Interfaces;
    public class ComponentHolder
    {
        public ComponentHolder(IComponent component)
        {
            Component = component;
            IsEnabled = false;
        }

        public IComponent Component { get; private set; }
        public bool IsEnabled { get; private set; }

        public bool Enable()
        {
            if (IsEnabled)
                return true;
            else if (Component.Enable())
                return IsEnabled = true;
            else
                return false;
        }

        public bool Disable()
        {
            if (!IsEnabled)
                return true;
            else if (Component.Disable())
                return IsEnabled = false;
            else
                return false;
        }
    }
}
