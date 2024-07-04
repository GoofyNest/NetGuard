using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CS0168

namespace NetGuardLoader.Functions
{
    public static class b64
    {
        public static string enc(string p)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(p));
        }

        public static string dec(string p)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(p));
        }

        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
        public static string bom(string plaintext, string pad)
        {
            var data = Encoding.UTF8.GetBytes(plaintext);
            var key = Encoding.UTF8.GetBytes(pad);

            return Convert.ToBase64String(data.Select((b, i) => (byte)(b ^ key[i % key.Length])).ToArray());
        }
        public static string dom(string enctext, string pad)
        {
            var data = Convert.FromBase64String(enctext);
            var key = Encoding.UTF8.GetBytes(pad);

            return Encoding.UTF8.GetString(data.Select((b, i) => (byte)(b ^ key[i % key.Length])).ToArray());
        }
    }

    public static class Hash
    {
        public static string read(string path)
        {
            try
            {
                if (!File.Exists(path)) return string.Empty;

                using (FileStream file = File.OpenRead(path))
                {
                    using (SHA1Managed checksum = new SHA1Managed())
                    {
                        return BitConverter.ToString(checksum.ComputeHash(file))
                            .Replace("-", string.Empty).ToLower();
                    }
                }
            }
            catch { }

            return string.Empty;
        }
    }
}