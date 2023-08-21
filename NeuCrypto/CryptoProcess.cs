using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private Logger logger = new Logger();
        private Encryptor encryptor = new Encryptor();

        public CryptoProcess()
        {
        }

        [ComVisible(true)]                
        public int InitAll(string logPath)
        {
            if (encryptor.Init(logPath) < 0)
            {
                LastError = encryptor.LastError;
                logger.LogMessage(Logger.LogLevel.Error, "InitAll: Encryptor.Init failed");
                return -1;
            }

            logger = encryptor.logger;
            logger.LogMessage(Logger.LogLevel.Debug, "InitAll: Success");

            return 0;
        }

        public string EncryptString(string szPlainText) => encryptor.EncryptTextAES(szPlainText);
       
        public string DecryptString(string szEncryptedText) => encryptor.DecryptTextAES(szEncryptedText);

        public string DecryptStringForDB(string szEncryptedText) => encryptor.DecryptTextAESForDB(szEncryptedText);
      
        public int BulkEncryptDBTable(string szSQLServer, string szDBNameOrPath, string szTableName, 
                                      string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            return encryptor.BulkEncryptDBTable(szSQLServer, szDBNameOrPath, szTableName, szFieldNames, szWhereClauseFields, szLstFilterOperators);
        }

        public int BulkDecryptDBTable(string szSQLServer, string szDBNameOrPath, string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators, string szAccessCode)
        {
            return encryptor.BulkDecryptDBTable(szSQLServer, szDBNameOrPath, szTableName, szFieldNames, szWhereClauseFields, szLstFilterOperators, szAccessCode);
        }

    }
}
