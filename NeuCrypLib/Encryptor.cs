using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace NeuCrypto
{
    public class Encryptor
    {
        public string LastError { get; set; }
        private RSAEncType rsaEncType = null;
        private AesEncType aesEncType = null;
        public const string EncryptDataHeader  = "_NDP_";
        private Logger logger = new Logger();

        public Encryptor()
        {
            rsaEncType = new RSAEncType();
            aesEncType = new AesEncType();
        }

        public int Init(Logger _logger)
        {
            logger = _logger;

            RegistryKey key1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Neudesic\\Neucrypto\\", false);

            if(key1 == null)
            {
                logger.LogMessage(Logger.LogLevel.Error, "Init: Registry key not found");
                return -1;
            }

            string certname = key1.GetValue("CertName").ToString();

            logger.LogMessage(Logger.LogLevel.Debug, "Init: CertName: " + certname);

            string value1 = key1.GetValue("Value1").ToString();

            key1.Close();

            if (InitRSA(certname) < 0)
            {
                logger.LogMessage(Logger.LogLevel.Error, "Init: InitRSA failed");
                return -1;
            }

            //string d = EncryptTextRSA(value1);
            string decryptedVal = DecryptTextRSA(value1);

            string key = decryptedVal + "ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF"; // 64 characters (256 bits), reading first 10 chars from registry
            string iv = decryptedVal + "ABCDEF0123456789ABCDEF"; // 32 characters (128 bits)

            if (InitAES(key, iv) < 0)
            {
                logger.LogMessage(Logger.LogLevel.Error, "Init: InitAES failed");
                return -1;
            }

            return 0;
        }

        public int InitRSA(string szCertSubjName = "")
        {
            if (szCertSubjName == "")
                szCertSubjName = "DOPCrypto";

            LastError = "";
            rsaEncType = new RSAEncType();
            int rc = rsaEncType.Init(szCertSubjName, true);

            if (rc < 0)
            {
                LastError = rsaEncType.LastError;
                logger.LogMessage(Logger.LogLevel.Error, "InitRSA: RSAEncType.Init failed: " + LastError);
            }

            return rc;
        }

        public int InitAES(string key, string IV)
        {
            LastError = "";
            aesEncType = new AesEncType();
            int rc = aesEncType.Init(key, IV);
            if (rc < 0)
                LastError = aesEncType.LastError;

            return rc;
        }

        public string EncryptTextRSA(string plainText)
        {
            if (String.IsNullOrEmpty(plainText)) return "";

            // If already encrypted, return as is
            if (plainText.StartsWith(EncryptDataHeader))
                return plainText;

            return EncryptDataHeader + rsaEncType.RSAEncryptText(plainText);
        }

        public string DecryptTextRSA(string szBase64EncData)
        {
            if(String.IsNullOrEmpty(szBase64EncData))
                return "";

            // If not encrypted, return as is
            if (!szBase64EncData.StartsWith(EncryptDataHeader))
                return szBase64EncData;

            szBase64EncData = szBase64EncData.Substring(EncryptDataHeader.Length);
            return rsaEncType.RSADecryptText(szBase64EncData);
        }

        public string EncryptTextAES(string plainText)
        {
            if (String.IsNullOrEmpty(plainText)) return "";

            // If already encrypted, return as is
            if(plainText.StartsWith(EncryptDataHeader))
                return plainText;

            return EncryptDataHeader + aesEncType.Encrypt(plainText);
        }

        public string DecryptTextAES(string szBase64EncData)
        {
            if (String.IsNullOrEmpty(szBase64EncData))
                return "";

            // If not encrypted, return as is
            if (!szBase64EncData.StartsWith(EncryptDataHeader))
                return szBase64EncData;

            szBase64EncData = szBase64EncData.Substring(EncryptDataHeader.Length);
            return aesEncType.Decrypt(szBase64EncData);
        }

        public string DecryptTextAESForDB(string szBase64EncData)
        {
            return DecryptTextAES(szBase64EncData).Replace("'", "''");
        }
    }
}
