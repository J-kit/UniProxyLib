using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UniProxy.Shared;
using UniProxy.Utilities;

namespace UniProxy.Modules
{
    /// <summary>
    /// Disclamer:
    ///     This is untested and relies on public documentations of the socks4 protocol
    /// </summary>
    internal class Socks4
    {
        private HostPortCollection _proxyInformation;
        public ProxyStatus Status { get; private set; }

        public Socks4(HostPortCollection proxyInformation)
        {
            _proxyInformation = proxyInformation;
        }

        public async Task<Stream> OpenStreamAsync(HostPortCollection dstSrv)
        {
            var socket = new TcpClient();
            if (await SwallowExceptionUtils.TryExec(
                    () => socket.ConnectAsync(_proxyInformation.Addr, _proxyInformation.Port)) && socket.Connected)
            {
                var buf = new byte[8];
                int bytesRead = 0;

                var nStream = socket.GetStream();
                var cmd = ProxyCommandBuildUtils.BuildSocks4Command(0x01, dstSrv); //SOCKS4_CMD_CONNECT

                await nStream.WriteAsync(cmd, 0, cmd.Length);

                bytesRead = await nStream.ReadAsync(buf, 0, buf.Length);
                if (bytesRead > 1 && buf[1] == 0x5A)
                {
                    return nStream;
                }
                else
                {
                    if (buf[1] == 0xFF)
                    {
                        //May be socks5?
                        Debugger.Break();
                    }
                    Status = ProxyStatus.ProtocolError;
                }
            }
            else
            {
                Status = ProxyStatus.ConnectionFail;
            }
            socket.Close();
            socket.Dispose();
            return null;
        }
    }
}