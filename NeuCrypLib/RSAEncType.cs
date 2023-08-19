using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeuCrypto
{
    internal class RSAEncType
    {
        public string LastError { get; set; }
        private SSLCert sslCert = null;
        private RSA rsapublicKey = null;
        private RSA rsaprivateKey = null;
        
        public RSAEncType()
        {
            LastError = "";
        }

        public int Init(string szCertSubjName, bool bLocalMachine)
        {
            LastError = "";
            sslCert = new SSLCert();
            int rc = sslCert.GetCertFromMyStore(szCertSubjName, bLocalMachine);
            if(rc < 0)
            {
                LastError = sslCert.LastError;
                return -1;
            }

            return GenerateRSAKeys();
        }

        public string RSAEncryptText(string plainText)
        {
            byte[] encData = RSAEncryptText(plainText, rsapublicKey);
            return Convert.ToBase64String(encData);
        }

        public string RSADecryptText(string szBase64EncData)
        {
            byte[] encryptedData = Convert.FromBase64String(szBase64EncData);
            return RSADecryptText(encryptedData, rsaprivateKey);
        }

        public byte[] RSAEncryptText(string plainText, X509Certificate2 certificate)
        {
            byte[] plainData = Encoding.UTF8.GetBytes(plainText);

            // Create a new instance of the RSACryptoServiceProvider
            using (RSA rsapublickey = certificate.GetRSAPublicKey())
            {
                return RSAEncryptText(plainText, rsapublickey);
            }
        }

        public string RSADecryptText(byte[] encryptedData, X509Certificate2 certificate)
        {
            // Create a new instance of the RSACryptoServiceProvider
            using (RSA rsaprivatekey = certificate.GetRSAPrivateKey())
            {
                return RSADecryptText(encryptedData, rsaprivatekey);
            }
        }

        public byte[] RSAEncryptText(string plainText, RSA rsaPublicKey)
        {
            byte[] plainData = Encoding.UTF8.GetBytes(plainText);

            // Use the RSA public key to encrypt the data
            byte[] encryptedData = rsaPublicKey.Encrypt(plainData, RSAEncryptionPadding.OaepSHA256);
            return encryptedData;
        }

        public string RSADecryptText(byte[] encryptedData, RSA rsaPrivateKey)
        {
            // Use the RSA private key to decrypt the data
            byte[] decryptedData = rsaPrivateKey.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
            string decryptedText = Encoding.UTF8.GetString(decryptedData);
            return decryptedText;
        }

        private int GenerateRSAKeys()
        {
            if (rsapublicKey == null)
            {
                rsapublicKey = sslCert.x509cert.GetRSAPublicKey();
            }

            if (rsaprivateKey == null)
            {
                try
                {
                    rsaprivateKey = sslCert.x509cert.GetRSAPrivateKey();
                }
                catch(Exception ex)
                {
                    LastError = "GenerateRSAKeys Failed: " + ex.Message;
                    return -1;
                }
            }

            return 0;
        }
    }
}
