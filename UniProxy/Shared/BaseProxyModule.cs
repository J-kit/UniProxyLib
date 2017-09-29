using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniProxyLib.Shared
{
    public class BaseProxyModule
    {
        public ProxyStatus Status { get; protected set; }
        protected readonly HostPortCollection _proxyInformation;

        public BaseProxyModule(HostPortCollection proxyInformation)
        {
            _proxyInformation = proxyInformation;
        }

        public virtual Task<Stream> OpenStreamAsync(HostPortCollection dstSrv)
        {
            return null;
        }
    }
}