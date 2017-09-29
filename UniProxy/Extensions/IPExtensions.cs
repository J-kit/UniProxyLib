using System.Linq;
using System.Net;

namespace UniProxyLib.Extensions
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