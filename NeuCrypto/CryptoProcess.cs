using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;

namespace NeuCrypto
{
    [ComVisible(true)]
    public class CryptoProcess
    {
        public string LastError { get; set; }
        private RSAEncType rsaEncType = null;
        private AesEncType aesEncType = null;

        public CryptoProcess()
        {
        }

        [ComVisible(true)]
        public void InitRSA(string szCertSubjName = "")
        {
            LastError = "";
            rsaEncType = new RSAEncType();
            rsaEncType.Init(szCertSubjName);
        }

        public void InitAES(string key, string IV)
        {
            LastError = "";
            aesEncType = new AesEncType();
            aesEncType.Init(key, IV);
        }


        public string EncryptTextRSA(string plainText)
        {
            return "NDP_" + rsaEncType.RSAEncryptText(plainText);
        }

        public string DecryptTextRSA(string szBase64EncData)
        {
            if(szBase64EncData.Substring(0, 4) != "NDP_")
                return szBase64EncData;

            szBase64EncData = szBase64EncData.Substring(4);
            return rsaEncType.RSADecryptText(szBase64EncData);
        }

        public string EncryptTextAES(string plainText)
        {
            return aesEncType.Encrypt(plainText);
        }

        public string DecryptTextAES(string szBase64EncData)
        {
            return aesEncType.Decrypt(szBase64EncData);
        }
    }
}
