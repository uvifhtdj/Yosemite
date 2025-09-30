using System.IO;
using System.Security.Cryptography;

namespace wrapper
{
    public static class AesUtil
    {
        public static byte[] AesEncrypt(byte[] input, byte[] pwd, int keySize = 128)
        {
            System.Security.Cryptography.AesCryptoServiceProvider aes = new System.Security.Cryptography.AesCryptoServiceProvider();
            aes.KeySize = keySize; // 设置密钥大小
            aes.Mode = System.Security.Cryptography.CipherMode.ECB; // 设置运算模式：ecb
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7; // 设置填充模式：PKCS7
            if (pwd != null && pwd.Length > 0)
            {
                aes.Key = pwd;
            }
            ICryptoTransform pCryptoTransform = aes.CreateEncryptor();
            byte[] resultBytes = pCryptoTransform.TransformFinalBlock(input, 0, input.Length);
            return resultBytes;
        }

        public static byte[] AesDecrypt(byte[] input, byte[] pwd, int keySize = 128)
        {
            System.Security.Cryptography.AesCryptoServiceProvider aes = new System.Security.Cryptography.AesCryptoServiceProvider();
            aes.KeySize = keySize; // 设置密钥大小
            aes.Mode = System.Security.Cryptography.CipherMode.ECB; // 设置运算模式：ecb
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7; // 设置填充模式：PKCS7
            if (pwd != null && pwd.Length > 0)
            {
                aes.Key = pwd;
            }
            ICryptoTransform pCryptoTransform = aes.CreateDecryptor();
            byte[] resultBytes = pCryptoTransform.TransformFinalBlock(input, 0, input.Length);
            return resultBytes;
        }
    }
}
