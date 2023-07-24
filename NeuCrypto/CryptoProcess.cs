using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NeuCrypto
{
    public class CryptoProcess
    {
        public string LastError { get; set; }
        private SSLCert sslCert = null;
        private RSA rsapublicKey = null;
        private RSA rsaprivateKey = null;
        public CryptoProcess()
        {
            LastError = "";
            sslCert = new SSLCert();    
        }

        public CryptoProcess(string szCertSubjectName)
        {
            LastError = "";
            sslCert = new SSLCert(szCertSubjectName);
            GenerateRSAKeys();
        }

        private void GenerateRSAKeys()
        {
            if (rsapublicKey == null)
            {
                rsapublicKey = sslCert.x509cert.GetRSAPublicKey();
            }
            if (rsaprivateKey == null)
            {
                rsaprivateKey = sslCert.x509cert.GetRSAPrivateKey();
            }
        }
        public byte[] RSAEncryptText(string plainText)
        {
            return RSAEncryptText(plainText, rsapublicKey);
        }

        public string RSADecryptText(byte[] encryptedData)
        {
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
    }
}
