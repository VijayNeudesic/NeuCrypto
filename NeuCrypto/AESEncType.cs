using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuCrypto
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal class AesEncType
    {
        private byte[] key;
        private byte[] iv;

        public AesEncType()
        {
            this.key = null;
            this.iv = null;
        }

        public void Init(string key, string iv)
        {
            if (key.Length != 64)
            {
                throw new ArgumentException("AES256 key must be 64 characters (256 bits) long.");
            }

            if (iv.Length != 32)
            {
                throw new ArgumentException("AES256 IV must be 32 characters (128 bits) long.");
            }

            this.key = StringToByteArray(key);
            this.iv = StringToByteArray(iv);
        }

        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes;

                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }

                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public string Decrypt(string encryptedText)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                string decryptedText;

                using (var msDecrypt = new System.IO.MemoryStream(cipherBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            decryptedText = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return decryptedText;
            }
        }

        private byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }

}
