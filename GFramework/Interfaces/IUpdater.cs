using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Interfaces
{
    using Enums;
    public interface IUpdater
    {
        uint Interval { get; }
        UpdaterMode Mode { get; }

        void Started();
        void Run();
        void Stopped();
    }
}
