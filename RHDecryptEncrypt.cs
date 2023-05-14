using System.Security.Cryptography;
using System.Text;

namespace RHTableTool
{
    public enum CryptType
    {
        XLSX = 0,
        XML = 1
    }
    public static class RHDecryptEncrypt
    {
        private static readonly Aes aes = Aes.Create();

        static RHDecryptEncrypt()
        {
            byte[] bytes = Encoding.UTF8.GetBytes("gkw3iurpamv;kj20984;asdkfjat1af");
            byte[] key = new byte[bytes.Length + 1];
            Buffer.BlockCopy(bytes, 0, key, 0, bytes.Length);

            aes.Key = key;
            aes.IV = new byte[] { 0xdb, 15, 0x49, 0x40, 0xdb, 15, 0x49, 0x40, 0xdb, 15, 0x49, 0x40, 0xdb, 15, 0x49, 0x40 };
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.Zeros;
        }

        public static byte[] Decrypt(byte[] toByte)
        {
            return aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(toByte, 0, toByte.Length);
        }

        public static byte[] Encrypt(byte[] toByte)
        {
            int x = toByte.Length % 16;
            if (x > 0)
            {
                x = 16 - x;
                byte[] newBytes = new byte[x + toByte.Length];
                Buffer.BlockCopy(toByte, 0, newBytes, 0, toByte.Length);
                newBytes[toByte.Length] = 0x2a;
                toByte = newBytes;
            }
            return aes.CreateEncryptor(aes.Key, aes.IV).TransformFinalBlock(toByte, 0, toByte.Length);
        }
    }
}
