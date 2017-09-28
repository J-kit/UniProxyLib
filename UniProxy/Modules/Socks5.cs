using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UniProxyLib.Shared;
using UniProxyLib.Utilities;

namespace UniProxyLib.Modules
{
    internal class Socks5
    {
        private static readonly byte[] Socks5Payload = new byte[] { 5, 1, 0x00 };

        public ProxyStatus Status { get; private set; }
        private readonly HostPortCollection _proxyInformation;

        public Socks5(HostPortCollection proxyInformation)
        {
            _proxyInformation = proxyInformation;
        }

        public async Task<Stream> OpenStreamAsync(HostPortCollection dstSrv)
        {
            var socket = new TcpClient();

            if (await SwallowExceptionUtils.TryExec(() => socket.ConnectAsync(_proxyInformation.Addr, _proxyInformation.Port)) && socket.Connected)
            {
                var buf = new byte[2];
                int bytesRead = 0;
                var nStream = socket.GetStream();

                await nStream.WriteAsync(Socks5Payload, 0, Socks5Payload.Length);
                bytesRead = await nStream.ReadAsync(buf, 0, 2);
                if (bytesRead != 2)
                {
                    Status = ProxyStatus.ProtocolError;
                    socket.Close();
                    socket.Dispose();
                }

                if (buf[0] == 0x05 && buf[1] == 0x00)
                {
                    //Continue...
                    var cmd = ProxyCommandBuildUtils.BuildSocks5Command(0x01, dstSrv); //0x01 = SOCKS5_CMD_CONNECT
                    await nStream.WriteAsync(cmd, 0, cmd.Length);
                    Array.Clear(cmd, 0, cmd.Length);
                    bytesRead = await nStream.ReadAsync(cmd, 0, cmd.Length);
                    if (bytesRead > 2 && cmd[1] == 0x00)
                    {
                        return nStream;
                    }
                    else
                    {
                        Status = ProxyStatus.ProtocolError;
                        Debugger.Break();
                    }
                }
                else
                {
                    Status = buf[0] == 0x02 ? ProxyStatus.PasswordRequired : ProxyStatus.ProtocolError;
                }
                nStream?.Close();
                nStream?.Dispose();
            }
            else
            {
                Status = ProxyStatus.ConnectionFail;
            }

            return null;
        }
    }
}