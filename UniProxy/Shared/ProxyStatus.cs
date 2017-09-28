using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniProxy.Shared
{
    public enum ProxyStatus
    {
        Default,
        Success,
        ConnectionFail,
        ProtocolError,
        PasswordRequired
    }
}