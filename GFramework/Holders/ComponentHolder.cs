using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Holders
{
    using Bases;
    using Interfaces;

    public class ComponentHolder
    {
        BaseLogger logger;

        public ComponentHolder(BaseLogger logger, IComponent component)
        {
            this.logger = logger;

            Component = component;
            IsEnabled = false;
        }

        public IComponent Component { get; private set; }
        public bool IsEnabled { get; private set; }

        public bool Enable()
        {
            if (IsEnabled)
            {
                logger.LogWarning("{0} has already enabled!", Component.GetType().Name);
                return true;
            }
            else if (Component.Enable())
            {
                logger.LogSuccess("{0} has enabled successfully!", Component.GetType().Name);
                IsEnabled = true;

                return true;
            }
            else
            {
                logger.LogError("{0} has been failed to enable!!", Component.GetType().Name);
                return false;
            }
        }

        public bool Disable()
        {
            if (!IsEnabled)
            {
                logger.LogWarning("{0} has already disabled!", Component.GetType().Name);
                return true;
            }
            else if (Component.Disable())
            {
                logger.LogSuccess("{0} has disabled successfuly!", Component.GetType().Name);
                IsEnabled = false;

                return true;
            }
            else
            {
                logger.LogError("{0} has been failed to disable!", Component.GetType().Name);
                return false;
            }
        }
    }
}
