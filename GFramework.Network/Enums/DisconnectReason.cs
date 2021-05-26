using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Enums
{
    public enum DisconnectReason
    {
        UserRequest,
        ServerShutdown,
        MaximumClientsReached,
        ConnectionFailed,
        Error,
        UnknownEvent,
    }
}
