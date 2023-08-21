using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace NeuCrypto
{
    public class EncryptDB_SQL : EncryptDB
    {
        public EncryptDB_SQL(Logger _logger, string szSQLServer, string szDBName) : base(_logger)
        {
            connString = "Driver={SQL Server};Server=" + szSQLServer + ";Database=" + szDBName + ";Trusted_Connection=yes;";
        }

        public override int UpdateDBTable(List<string> distinctQueries, OdbcConnection connection)
        {
            try
            {
                using (OdbcCommand updateCommand = connection.CreateCommand())
                {
                    // Set the batch update size
                    int batchSize = 200;
                    int completed = 0;
                    int currentBatchSize = 0;

                    foreach (string query in distinctQueries)
                    {
                        updateCommand.CommandText += query + "; ";
                        currentBatchSize++;

                        if (currentBatchSize >= batchSize)
                        {
                            completed += currentBatchSize;
                            logger.LogMessage(Logger.LogLevel.Debug, $"UpdateDBTable: Updated {completed}/{distinctQueries.Count} queries.");
                            updateCommand.ExecuteNonQuery();
                            updateCommand.CommandText = "";
                            currentBatchSize = 0;
                        }
                    }

                    // Execute any remaining batched updates
                    if (!string.IsNullOrEmpty(updateCommand.CommandText))
                    {
                        completed += currentBatchSize;
                        logger.LogMessage(Logger.LogLevel.Debug, $"UpdateDBTable: Updated {completed}/{distinctQueries.Count} queries.");
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogMessage(Logger.LogLevel.Error, $"UpdateDBTable: {ex.Message}");
                return -1;
            }

            return 0;
        }
        public override string ConstructWhereClause(Dictionary<string, Tuple<Type, string>> whereClauseFields, string szLstFilterOperators)
        {
            string szRet = "";
            string[] lstFilterOperators = szLstFilterOperators.Split(',');
            int i = 0;
            foreach (KeyValuePair<string, Tuple<Type, string>> fldToEnc in whereClauseFields)
            {
                if (szRet != "")
                    szRet += " " + lstFilterOperators[i++] + " ";

                szRet += fldToEnc.Key + "=" + FormatField(fldToEnc.Value.Item1, fldToEnc.Value.Item2);
            }

            return szRet;
        }

        public override string FormatField(Type fldType, string szFieldValue)
        {
            string szRet = "";
            switch (fldType.Name)
            {
                case "Int32":
                    szRet = szFieldValue;
                    break;
                case "String":
                    szFieldValue = szFieldValue.Replace("'", "''");
                    szRet = "'" + szFieldValue + "'";
                    break;
                case "DateTime":
                    szRet = "'" + szFieldValue + "'";
                    break;
            }

            return szRet;
        }
    }
}
