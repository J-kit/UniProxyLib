using System.Text;

namespace UniProxyLib.Extensions
{
    public static class StringExtensions
    {
        public static string GetString(this byte[] input)
            => Encoding.UTF8.GetString(input);

        public static byte[] GetBytes(this string input, Encoding enc = null)
            => enc?.GetBytes(input) ?? Encoding.UTF8.GetBytes(input);
    }
}