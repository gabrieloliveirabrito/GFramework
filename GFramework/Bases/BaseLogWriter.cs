using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Bases
{
    using Enums;

    public abstract class BaseLogWriter
    {
        public abstract void Write(BaseLog log);
    }
}
