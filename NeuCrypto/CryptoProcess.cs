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
using Serilog;

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
        public int InitAll(string logPath, bool bCertStoreLocalMachine)
        {
            logger.InitLogs(logPath);

            if (encryptor.Init(logger, bCertStoreLocalMachine) < 0)
            {
                LastError = encryptor.LastError;
                logger.LogMessage(Logger.LogLevel.Error, "InitAll: Encryptor.Init failed");
                return -1;
            }

            logger.LogMessage(Logger.LogLevel.Debug, "InitAll: Success");

            return 0;
        }

        public string EncryptString(string szPlainText)
        {
            return encryptor.EncryptTextAES(szPlainText);
        }
       
        public string DecryptString(string szEncryptedText)
        {
            return encryptor.DecryptTextAES(szEncryptedText);
        }
      
        public int BulkEncryptAccessDBTable(string szDBPath, string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            DateTime start = DateTime.Now;
            EncryptDB encryptDB = new EncryptDB(logger, encryptor);
            if (encryptDB.BulkEncryptAccessDBTable(szDBPath, szTableName, szFieldNames, szWhereClauseFields, szLstFilterOperators) < 0)
            {
                LastError = encryptDB.LastError;
                logger.LogMessage(Logger.LogLevel.Error, "BulkEncryptAccessDBTable: Failed");
                return -1;
            }

            TimeSpan timeDiff = DateTime.Now - start;
            logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptAccessDBTable: {encryptDB.rows_updated}/{encryptDB.row_count} rows updated in {timeDiff.TotalSeconds} seconds");

            return 0;
        }

        public int BulkDecryptAccessDBTable(string szDBPath, string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            DateTime start = DateTime.Now;
            EncryptDB encryptDB = new EncryptDB(logger, encryptor);
            if (encryptDB.BulkDecryptAccessDBTable(szDBPath, szTableName, szFieldNames, szWhereClauseFields, szLstFilterOperators) < 0)
            {
                LastError = encryptDB.LastError;
                logger.LogMessage(Logger.LogLevel.Error, "BulkDecryptAccessDBTable: Failed");
                return -1;
            }

            TimeSpan timeDiff = DateTime.Now - start;
            logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptAccessDBTable: {encryptDB.rows_updated}/{encryptDB.row_count} rows updated in {timeDiff.TotalSeconds} seconds");

            return 0;
        }
    }
}
