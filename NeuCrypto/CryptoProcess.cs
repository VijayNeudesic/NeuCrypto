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
        public string StatusMsg { get; set; }
        private Logger logger = new Logger();
        private Encryptor encryptor = new Encryptor();

        public CryptoProcess()
        {
        }

        [ComVisible(true)]                
        public int InitAll(string logPath)
        {
            logger.InitLogs(logPath);

            if (encryptor.Init(logger) < 0)
            {
                LastError = encryptor.LastError;
                logger.LogMessage(Logger.LogLevel.Error, "InitAll: Encryptor.Init failed");
                return -1;
            }

            logger.LogMessage(Logger.LogLevel.Debug, "InitAll: Success");

            return 0;
        }

        public string EncryptString(string szPlainText) => encryptor.EncryptTextAES(szPlainText);
       
        public string DecryptString(string szEncryptedText) => encryptor.DecryptTextAES(szEncryptedText);

        public string DecryptStringForDB(string szEncryptedText) => encryptor.DecryptTextAESForDB(szEncryptedText);
      
        public int BulkEncryptDBTable(string szSQLServer, string szDBNameOrPath, string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            DateTime start = DateTime.Now;
            EncryptDB encryptDB;

            if (szSQLServer.Length > 0)
                encryptDB = new EncryptDB_SQL(logger, encryptor, szSQLServer, szDBNameOrPath);
            else
                encryptDB = new EncryptDB_Access(logger, encryptor, szDBNameOrPath);

            if (encryptDB.BulkEncryptDBTable(szTableName, szFieldNames, szWhereClauseFields, szLstFilterOperators) < 0)
            {
                LastError = encryptDB.LastError;
                logger.LogMessage(Logger.LogLevel.Error, "BulkEncryptDBTable: Failed");
                return -1;
            }

            TimeSpan timeDiff = DateTime.Now - start;
            
            StatusMsg = $"Completed: {encryptDB.rows_updated}/{encryptDB.row_count} rows updated in {timeDiff.TotalSeconds} seconds";
            logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptDBTable: {StatusMsg}");

            return 0;
        }

        public int BulkDecryptDBTable(string szSQLServer, string szDBNameOrPath, string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            DateTime start = DateTime.Now;
            EncryptDB encryptDB;

            if (szSQLServer.Length > 0)
                encryptDB = new EncryptDB_SQL(logger, encryptor, szSQLServer, szDBNameOrPath);
            else
                encryptDB = new EncryptDB_Access(logger, encryptor, szDBNameOrPath);

            if (encryptDB.BulkDecryptDBTable(szTableName, szFieldNames, szWhereClauseFields, szLstFilterOperators) < 0)
            {
                LastError = encryptDB.LastError;
                logger.LogMessage(Logger.LogLevel.Error, "BulkDecryptDBTable: Failed");
                return -1;
            }

            TimeSpan timeDiff = DateTime.Now - start;
            StatusMsg = $"Completed: {encryptDB.rows_updated}/{encryptDB.row_count} rows updated in {timeDiff.TotalSeconds} seconds";
            logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptDBTable: {StatusMsg}");

            return 0;
        }

        public string GenerateAccessCode() => encryptor.GenerateAccessCode();
    }
}
