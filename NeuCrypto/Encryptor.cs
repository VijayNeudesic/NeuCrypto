using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuCrypto
{
    internal class Encryptor
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

        public int Init(Logger _logger, bool bCertStoreLocalMachine)
        {
            logger = _logger;

            if (InitRSA("DOPCrypto", bCertStoreLocalMachine) < 0)
            {
                logger.LogMessage(Logger.LogLevel.Error, "Init: InitRSA failed");
                return -1;
            }

            string key = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF"; // 64 characters (256 bits)
            string iv = "0123456789ABCDEF0123456789ABCDEF"; // 32 characters (128 bits)

            if (InitAES(key, iv) < 0)
            {
                logger.LogMessage(Logger.LogLevel.Error, "Init: InitAES failed");
                return -1;
            }

            return 0;
        }

        public int InitRSA(string szCertSubjName = "", bool bLocalMachine = false)
        {
            if (szCertSubjName == "")
                szCertSubjName = "DOPCrypto";

            LastError = "";
            rsaEncType = new RSAEncType();
            int rc = rsaEncType.Init(szCertSubjName, bLocalMachine);

            if (rc < 0)
                LastError = rsaEncType.LastError;

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
            return EncryptDataHeader + rsaEncType.RSAEncryptText(plainText);
        }

        public string DecryptTextRSA(string szBase64EncData)
        {
            if (szBase64EncData.Substring(0, EncryptDataHeader.Length) != EncryptDataHeader)
                return szBase64EncData;

            szBase64EncData = szBase64EncData.Substring(EncryptDataHeader.Length);
            return rsaEncType.RSADecryptText(szBase64EncData);
        }

        public string EncryptTextAES(string plainText)
        {
            if (String.IsNullOrEmpty(plainText)) return "";

            return EncryptDataHeader + aesEncType.Encrypt(plainText);
        }

        public string DecryptTextAES(string szBase64EncData)
        {
            if (szBase64EncData.Substring(0, EncryptDataHeader.Length) != EncryptDataHeader)
                return szBase64EncData;

            szBase64EncData = szBase64EncData.Substring(EncryptDataHeader.Length);
            return aesEncType.Decrypt(szBase64EncData);
        }
    }
}
