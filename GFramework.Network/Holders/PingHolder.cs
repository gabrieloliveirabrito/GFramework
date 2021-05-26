using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Holders
{
    public class PingHolder
    {
        public PingHolder(int length)
        {
            PingBuffer = new byte[length];
            PingHandled = 0;
        }

        public byte[] PingBuffer { get; set; }
        public int PingHandled { get; set; }
    }
}
