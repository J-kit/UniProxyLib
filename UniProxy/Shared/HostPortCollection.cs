using System.Net;
using UniProxyLib.Extensions;
using UniProxyLib.Utilities;

namespace UniProxyLib.Shared
{
    public class HostPortCollection
    {
        public string Host;
        private IPAddress _addr;
        public int Port;

        public IPAddress Addr
        {
            get
            {
                _addr = _addr ?? Host.Resolve();
                return _addr;
            }
            set
            {
                _addr = value;
            }
        }

        public HostPortCollection(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public HostPortCollection(IPAddress addr, int port)
        {
            Addr = addr;
            Port = port;
        }

        public bool HostAvaiable => !string.IsNullOrEmpty(Host);
        public bool UseHost = true;

        public bool UseAvaiableHost => HostAvaiable && UseHost;
    }
}