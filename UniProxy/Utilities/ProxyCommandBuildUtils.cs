using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UniProxyLib.Shared;
using UniProxyLib.Shared.Exceptions;

namespace UniProxyLib.Utilities
{
    internal class ProxyCommandBuildUtils
    {
        private const string HTTP_PROXY_CONNECT_CMD = "CONNECT {0}:{1} HTTP/1.0\r\nHost: {0}:{1}\r\n\r\n";

        public static byte[] GenerateHttpConnectCommand(string host, int port)
        {
            string connectCmd = String.Format(CultureInfo.InvariantCulture, HTTP_PROXY_CONNECT_CMD, host, port.ToString(CultureInfo.InvariantCulture));
            return connectCmd.GetBytes();
        }

        public static byte[] BuildSocks4Command(byte command, HostPortCollection dstHost)
        {
            dstHost.UseHost = false; // 4A Doesnt work
            byte[] destAddr = dstHost.UseAvaiableHost ? new byte[] { 0, 0, 0, 1 } : dstHost.Addr.GetAddressBytes();

            byte[] destPort = new byte[] { Convert.ToByte(dstHost.Port / 256), Convert.ToByte(dstHost.Port % 256) };
            byte[] userIdBytes = string.Empty.GetBytes(Encoding.ASCII);

            byte[] hostBytes = dstHost.UseAvaiableHost ? dstHost.Host.GetBytes(Encoding.ASCII) : null;
            var rqLen = dstHost.UseAvaiableHost && hostBytes != null ? 10 + userIdBytes.Length + hostBytes.Length : 9 + userIdBytes.Length;

            byte[] request = new byte[rqLen];

            //  set the bits on the request byte array
            request[0] = 0x04;//SOCKS4_VERSION_NUMBER
            request[1] = command;
            destPort.CopyTo(request, 2);
            destAddr.CopyTo(request, 4);
            userIdBytes.CopyTo(request, 8);  // copy the userid to the request byte array
            request[8 + userIdBytes.Length] = 0x00;  // null (byte with all zeros) terminator for userId
            if (dstHost.UseAvaiableHost && hostBytes != null)
            {
                hostBytes.CopyTo(request, 9 + userIdBytes.Length);  // copy the host name to the request byte array
                request[9 + userIdBytes.Length + hostBytes.Length] = 0x00;  // null (byte with all zeros) terminator for userId
            }

            return request;
        }

        public static byte[] BuildSocks5Command(byte command, HostPortCollection dstHost)
        {
            byte addressType = dstHost.UseAvaiableHost ? GetDstAddrType(dstHost.Host) : GetDstAddrType(dstHost.Addr);
            byte[] destAddr = dstHost.UseAvaiableHost ? GetHostBytes(dstHost.Host) : dstHost.Addr.GetAddressBytes();
            byte[] destPort = new byte[] { Convert.ToByte(dstHost.Port / 256), Convert.ToByte(dstHost.Port % 256) };

            byte[] request = new byte[4 + destAddr.Length + 2];
            request[0] = 5; //SOCKS5_VERSION_NUMBER
            request[1] = command;
            request[2] = 0x00; //SOCKS5_RESERVED
            request[3] = addressType;
            destAddr.CopyTo(request, 4);
            destPort.CopyTo(request, 4 + destAddr.Length);

            return request;
        }

        private static byte[] GetHostBytes(string host)
        {
            byte[] bytes = new byte[host.Length + 1];
            bytes[0] = Convert.ToByte(host.Length);
            host.GetBytes(Encoding.ASCII).CopyTo(bytes, 1);
            return bytes;
        }

        private static byte GetDstAddrType(string host)
        {
            IPAddress ipAddr;
            return IPAddress.TryParse(host, out ipAddr) ? GetDstAddrType(ipAddr) : (byte)0x03; //SOCKS5_ADDRTYPE_DOMAIN_NAME
        }

        private static byte GetDstAddrType(IPAddress ipAddr)
        {
            switch (ipAddr.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    return 0X01; //SOCKS5_ADDRTYPE_IPV4

                case AddressFamily.InterNetworkV6:
                    return 0X04; //SOCKS5_ADDRTYPE_IPV6

                default:
                    throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "The host addess {0} of type '{1}' is not a supported address type.  The supported types are InterNetwork and InterNetworkV6.", ipAddr, Enum.GetName(typeof(AddressFamily), ipAddr.AddressFamily)));
            }
        }
    }
}