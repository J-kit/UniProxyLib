﻿using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UniProxyLib.Shared;
using UniProxyLib.Utilities;

namespace UniProxyLib.Modules
{
    /// <summary>
    /// Disclamer:
    ///     This is untested and relies on public documentations of the socks4 protocol
    /// </summary>
    internal class Socks4 : BaseProxyModule
    {
        public Socks4(HostPortCollection proxyInformation) : base(proxyInformation)
        {
        }

        public override async Task<Stream> OpenStreamAsync(HostPortCollection dstSrv)
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