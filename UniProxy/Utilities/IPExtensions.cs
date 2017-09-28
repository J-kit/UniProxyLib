using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UniProxy.Utilities
{
    internal static class IpExtensions
    {
        internal static IPAddress Resolve(this string addr)
        {
            IPAddress ipAddress;
            return IPAddress.TryParse(addr, out ipAddress) ? ipAddress : Dns.GetHostAddresses(addr).FirstOrDefault();
        }
    }
}