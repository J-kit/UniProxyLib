using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UniProxyLib.Shared;
using UniProxyLib.Utilities;

namespace UniProxyLib.Modules
{
    public class HttpConnect : BaseProxyModule
    {
        public HttpConnect(HostPortCollection proxyInformation) : base(proxyInformation)
        {
        }

        public override async Task<Stream> OpenStreamAsync(HostPortCollection dstSrv)
        {
            var socket = new TcpClient();
            if (await SwallowExceptionUtils.TryExec(() => socket.ConnectAsync(_proxyInformation.Addr, _proxyInformation.Port)) && socket.Connected)
            {
                var nStream = socket.GetStream();
                var cmd = ProxyCommandBuildUtils.GenerateHttpConnectCommand(dstSrv.Host, dstSrv.Port);

                await nStream.WriteAsync(cmd, 0, cmd.Length);
                return nStream;
            }
            return null;
        }
    }
}