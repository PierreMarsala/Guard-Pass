using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Shapes;

namespace Guard_Pass.Methods
{
    public static class KeyManager
    {
        public static byte[] KeyGenerator(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static byte[] IVGenerator()
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateIV();
                return aesAlg.IV;
            }
        }

        public static (byte[], byte[]) Separator(byte[] text)
        {
            byte[] result = new byte[text.Length - 16];
            byte[] iv = new byte[16];
            Array.Copy(text, iv, 16);
            Array.Copy(text, 16, result, 0, result.Length);

            return (iv, result);
        }

        public static void DataToFile(string filePath, byte[] data, byte[] iv)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Write(iv, 0, iv.Length);
                fileStream.Write(data, 0, data.Length);
            }
        }
    }
}
