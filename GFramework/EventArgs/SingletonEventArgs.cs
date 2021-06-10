using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.EventArgs
{
    using Interfaces;

    public class SingletonEventArgs : System.EventArgs
    {
        public ISingleton Singleton { get; private set; }

        public SingletonEventArgs(ISingleton singleton)
        {
            Singleton = singleton;
        }
    }
}
