using System;
using System.IO;
using System.Text;

namespace wrapper
{
    internal static class Extensions
    {
        public static DateTime DateTimeNow(this object obj)
        {
            return DateTime.Now.AddHours(8);
        }

        public static string GetPathInPrj(this string relativePath)
        {
            var dir = new DirectoryInfo("..\\..\\").FullName;
            return dir + "\\" + relativePath;
        }

        public static string GetPathInBin(this string relativePath)
        {
            var dir = new DirectoryInfo(".").FullName;
            return dir + "\\" + relativePath;
        }

        /// <summary>
        /// 将16进制字符串转换为字节数组
        /// </summary>
        /// <param name="hexString">16进制字符</param>
        /// <returns></returns>
        public static byte[] ToHexArray(this string hexString)
        {
            if (hexString == null)
            {
                return new byte[0];
            }
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("str长度必须为2的倍数!");
            }
            byte[] result = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                string buf = hexString.Substring(i, 2);
                result[i / 2] = Convert.ToByte(buf, 16);
            }

            return result;
        }

        /// <summary>
        /// 将字节数组转换为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="isHexUpper">16进制字符串是否为大写，默认为大写显示</param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes, bool isHexUpper = true)
        {
            string convertString = "x2";
            if (isHexUpper)
            {
                convertString = "X2";
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString(convertString));
            }
            return builder.ToString();
        }
    }
}
