using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuCrypto
{
    public class EncryptDB_Access : EncryptDB
    {
        public EncryptDB_Access(Logger _logger, string DBPath) : base(_logger)
        {
            connString = $"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={DBPath};";
        }

        public override int UpdateDBTable(List<string> distinctQueries, OdbcConnection connection)
        {
            try
            {
                int completed = 0;
                foreach (string query in distinctQueries)
                {
                    completed++;
                    using (OdbcCommand updateCommand = new OdbcCommand(query, connection))
                    {
                        if(BatchSize <= 0 || completed % BatchSize == 0)
                            logger.LogMessage(Logger.LogLevel.Debug, $"UpdateDBTable: Updated {completed}/{distinctQueries.Count} queries.");
                        updateCommand.ExecuteNonQuery();
                    }
                }
                logger.LogMessage(Logger.LogLevel.Debug, $"UpdateDBTable: Updated {completed}/{distinctQueries.Count} queries.");
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
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

                szRet += " [" + fldToEnc.Key + "] =" + FormatField(fldToEnc.Value.Item1, fldToEnc.Value.Item2);
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
                    szRet = "#" + szFieldValue + "#";
                    break;
            }

            return szRet;
        }


    }
}
