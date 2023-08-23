using Serilog.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuCrypto
{
    public class EncryptDB
    {
        public string LastError { get; set; }
        public Encryptor encryptor { get; set;}

        protected string connString = "";
        protected Logger logger = new Logger();

        public int row_count = 0;
        public int rows_updated = 0;
        public int BatchSize = 0;

        public EncryptDB(Logger logger)
        {
            this.logger = logger;
        }

        public int BulkEncryptDBTable(string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptDBTable: {connString}, {szTableName}, {szFieldNames}, {szWhereClauseFields}, {szLstFilterOperators}");

            //Split field names into a dictonary where field name is the key and field type is the value
            //format: id,name,dob
            Dictionary<string, string> encfieldNames = szFieldNames.Split(',').ToDictionary(szField => szField, szField => "");

            //Split row keys into a dictonary where field name is the key and field type is the value
            //format:  Id,name,dob
            Dictionary<string, Tuple<Type, string>> whereClauseFields = szWhereClauseFields.Split(',')
                                                          .ToDictionary(szFieldParts => szFieldParts, szFieldParts => new Tuple<Type, string>(null, ""));


            string updateQuery = "";
            row_count = 0;
            rows_updated = 0;

            Dictionary<string, Tuple<Type, int>> ColumnTypeAndSize = new Dictionary<string, Tuple<Type, int>>();
            List<String> updateQueries = new List<string>();

            // Create a new OleDbConnection using the connection string
            using (OdbcConnection connection = new OdbcConnection(connString))
            {
                try
                {
                    // Open the database connection
                    connection.Open();

                    // Create a command to read the data
                    string selectQuery = "SELECT * FROM [" + szTableName + "]";

                    logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptDBTable: {selectQuery}");

                    using (OdbcCommand selectCommand = new OdbcCommand(selectQuery, connection))
                    {
                        /////////////////////////////////////////////////////////////////////
                        //  Get the schema of the table to get the column types and sizes
                        /////////////////////////////////////////////////////////////////////
                        OdbcDataReader reader = selectCommand.ExecuteReader();
                        DataTable schemaTable = reader.GetSchemaTable();
                        logger.LogMessage(Logger.LogLevel.Debug, "Schema retrieved.");
                        foreach (DataRow row in schemaTable.Rows)
                            ColumnTypeAndSize.Add(row["ColumnName"].ToString().ToUpper(), Tuple.Create((Type)row["DataType"], (int)row["ColumnSize"]));

                        logger.LogMessage(Logger.LogLevel.Debug, $"Column type size: {ColumnTypeAndSize.Count}.");

                        Dictionary<string,int> dictColsToResize = new Dictionary<string, int>();
                        // Check if the reader has rows
                        if (reader.HasRows)
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////
                            //  Read through the record sets to see if the encrypted value is larger than the column size
                            /////////////////////////////////////////////////////////////////////////////////////////////////
                            while (reader.Read())
                            {
                                //Validate that the encrypted value is not larger than the column size
                                foreach (var encfield in encfieldNames)
                                {
                                    string key = encfield.Key;
                                    object fieldValue = reader[key];

                                    if (fieldValue == null || fieldValue == DBNull.Value || fieldValue.ToString().StartsWith(Encryptor.EncryptDataHeader))
                                        continue;

                                    row_count++;

                                    string encryptedValue = encryptor.EncryptTextAES(fieldValue.ToString());

                                    var typeAndSize = ColumnTypeAndSize[key.ToUpper()];
                                    if (encryptedValue.Length > typeAndSize.Item2)
                                    {
                                        if(dictColsToResize.ContainsKey(key))
                                        {
                                            if (dictColsToResize[key] < encryptedValue.Length)
                                                dictColsToResize[key] = encryptedValue.Length;
                                        }
                                        else
                                            dictColsToResize[key] = encryptedValue.Length;
                                    }
                                }
                            }

                            reader.Close();

                            if (dictColsToResize.Count > 0)
                            {
                                string szColsToResize = dictColsToResize.Aggregate("", (current, col) => current + $" {col.Key}({col.Value}),");

                                if(szColsToResize.EndsWith(","))
                                    szColsToResize = szColsToResize.Substring(0, szColsToResize.Length - 1);

                                LastError = $"Resize: {szColsToResize}";
                                logger.LogMessage(Logger.LogLevel.Error, LastError);
                                connection.Close();
                                return -1;
                            }

                            if (row_count > 0)
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////
                                // If there are rows that need to be encrypted, then create the update query
                                /////////////////////////////////////////////////////////////////////////////////////////////////

                                //restart the reader and the row count
                                row_count = 0;
                                rows_updated = 0;

                                logger.LogMessage(Logger.LogLevel.Debug, "All column sizes are good.");
                                reader = selectCommand.ExecuteReader();

                                logger.LogMessage(Logger.LogLevel.Debug, $"Real encryption starts now :).");

                                // Loop through the rows
                                while (reader.Read())
                                {
                                    bool bPerformUpdate = false;
                                    bool bFldsEmpty = true;
                                    row_count++;

                                    //Had to use the classic for loop to avoid the exception of collection been modified
                                    for (int i = 0; i < encfieldNames.Count; i++)
                                    {
                                        string key = encfieldNames.ElementAt(i).Key;
                                        object fieldValue = reader[key];
                                        encfieldNames[key] = (fieldValue == DBNull.Value) ? "" : fieldValue.ToString();

                                        // Encrypt the field value if it is not null
                                        if (fieldValue != DBNull.Value && !fieldValue.ToString().StartsWith(Encryptor.EncryptDataHeader))
                                        {
                                            bPerformUpdate = true;
                                            string encryptedValue = encryptor.EncryptTextAES(fieldValue.ToString());
                                            encfieldNames[key] = encryptedValue;
                                        }

                                        if (fieldValue != DBNull.Value && !string.IsNullOrEmpty(fieldValue.ToString()))
                                            bFldsEmpty = false;
                                    }

                                    // If there is no fields that were encrypted, then continue
                                    if (!bPerformUpdate || bFldsEmpty)
                                        continue;

                                    rows_updated++;

                                    // Loop through the where clause fields
                                    for (int i = 0; i < whereClauseFields.Count; i++)
                                    {
                                        string key = whereClauseFields.ElementAt(i).Key;
                                        // Get the value of the field
                                        object fieldValue = reader[key];
                                        var TypeAndSize = ColumnTypeAndSize[key.ToUpper()];
                                        whereClauseFields[key] = new Tuple<Type, string>(TypeAndSize.Item1, fieldValue.ToString());
                                    }

                                    // Update the row with the encrypted values
                                    updateQuery = "UPDATE [" + szTableName + "] SET ";
                                    foreach (KeyValuePair<string, string> fldToEnc in encfieldNames)
                                    {
                                        if(string.IsNullOrEmpty(fldToEnc.Value))
                                            continue;

                                        var TypeAndSize = ColumnTypeAndSize[fldToEnc.Key.ToUpper()];
                                        updateQuery += " [" + fldToEnc.Key + "] =" + FormatField(TypeAndSize.Item1, fldToEnc.Value) + ",";
                                    }
                                    updateQuery = updateQuery.TrimEnd(',');
                                    updateQuery += " WHERE " + ConstructWhereClause(whereClauseFields, szLstFilterOperators);
                                    /*using (OdbcCommand updateCommand = new OdbcCommand(updateQuery, connection))
                                    {
                                        updateCommand.ExecuteNonQuery();
                                    }*/

                                    //if(!updateQueries.Contains(updateQuery))
                                    updateQueries.Add(updateQuery);

                                    if (rows_updated % 1000 == 0)
                                        logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptDBTable: {rows_updated} rows added. SQL: {updateQuery}");

                                }

                                reader.Close();

                                logger.LogMessage(Logger.LogLevel.Debug, "Last Update query: " + updateQuery);

                                logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptDBTable: {updateQueries.Count} in list.");
                                List<string> distinctQueries = updateQueries.Distinct().ToList();
                                logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptDBTbl: {distinctQueries.Count} distinct queries.");

                                //Now update the rows
                                if(UpdateDBTable(distinctQueries, connection) < 0)
                                {
                                    logger.LogMessage(Logger.LogLevel.Error, $"BulkEncryptDBTable: {LastError}");
                                    return -1;
                                }   

                                logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptDBTable: {updateQueries.Count} update queries executed.");
                            }
                            else
                            {
                                logger.LogMessage(Logger.LogLevel.Debug, "No rows to encrypt.");
                            }

                            connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    // Handle any exceptions here
                    LastError = ex.Message;
                    logger.LogMessage(Logger.LogLevel.Error, $"BulkEncryptDBTable: {ex.Message} : UpdateQuery: {updateQuery}");
                    return -1;
                }

            }

            return 0;
        }

        public int BulkDecryptDBTable(string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptDBTable: {connString}, {szTableName}, {szFieldNames}, {szWhereClauseFields}, {szLstFilterOperators}");

            //Split field names into a dictonary where field name is the key and field type is the value
            //format: id,name,dob
            Dictionary<string, string> encfieldNames = szFieldNames.Split(',').ToDictionary(szField => szField, szField => "");

            //Split row keys into a dictonary where field name is the key and field type is the value
            //format:  Id,name,dob
            Dictionary<string, Tuple<Type, string>> whereClauseFields = szWhereClauseFields.Split(',')
                                                          .ToDictionary(szFieldParts => szFieldParts, szFieldParts => new Tuple<Type, string>(null, ""));


            string updateQuery = "";
            row_count = 0;
            rows_updated = 0;

            Dictionary<string, Tuple<Type, int>> ColumnTypeAndSize = new Dictionary<string, Tuple<Type, int>>();
            List<string> updateQueries = new List<string>();

            // Create a new OleDbConnection using the connection string
            using (OdbcConnection connection = new OdbcConnection(connString))
            {
                try
                {
                    // Open the database connection
                    connection.Open();

                    // Create a command to read the data
                    string selectQuery = "SELECT * FROM [" + szTableName + "]";

                    logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptDBTable: {selectQuery}");

                    using (OdbcCommand selectCommand = new OdbcCommand(selectQuery, connection))
                    {
                        /////////////////////////////////////////////////////////////////////
                        //  Get the schema of the table to get the column types and sizes
                        /////////////////////////////////////////////////////////////////////
                        OdbcDataReader reader = selectCommand.ExecuteReader();
                        DataTable schemaTable = reader.GetSchemaTable();
                        logger.LogMessage(Logger.LogLevel.Debug, "Schema retrieved.");
                        foreach (DataRow row in schemaTable.Rows)
                            ColumnTypeAndSize.Add(row["ColumnName"].ToString().ToUpper(), Tuple.Create((Type)row["DataType"], (int)row["ColumnSize"]));

                        logger.LogMessage(Logger.LogLevel.Debug, $"Column type size: {ColumnTypeAndSize.Count}.");

                        // Check if the reader has rows
                        if (reader.HasRows)
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////
                            // If there are rows that need to be encrypted, then create the update query
                            /////////////////////////////////////////////////////////////////////////////////////////////////

                            //restart the reader and the row count
                            row_count = 0;
                            rows_updated = 0;

                            reader.Close();
                            reader = selectCommand.ExecuteReader();

                            logger.LogMessage(Logger.LogLevel.Debug, $"Real decryption starts now :).");

                            // Loop through the rows
                            while (reader.Read())
                            {
                                bool bPerformUpdate = false;
                                bool bFldsEmpty = true;
                                row_count++;

                                //Had to use the classic for loop to avoid the exception of collection been modified
                                for (int i = 0; i < encfieldNames.Count; i++)
                                {
                                    string key = encfieldNames.ElementAt(i).Key;
                                    object fieldValue = reader[key];
                                    encfieldNames[key] = (fieldValue == DBNull.Value) ? "" : fieldValue.ToString();

                                    // Decrypt the field value if it is not null and it starts with the encrypted data header
                                    if (fieldValue != DBNull.Value && fieldValue.ToString().StartsWith(Encryptor.EncryptDataHeader))
                                    {
                                        bPerformUpdate = true;
                                        string encryptedValue = encryptor.DecryptTextAES(fieldValue.ToString());
                                        encfieldNames[key] = encryptedValue;
                                    }

                                    if (fieldValue != DBNull.Value && !string.IsNullOrEmpty(fieldValue.ToString()))
                                        bFldsEmpty = false;
                                }

                                // If there is no fields that were encrypted, then continue
                                if (!bPerformUpdate || bFldsEmpty)
                                    continue;

                                rows_updated++;

                                // Loop through the where clause fields
                                for (int i = 0; i < whereClauseFields.Count; i++)
                                {
                                    string key = whereClauseFields.ElementAt(i).Key;
                                    // Get the value of the field
                                    object fieldValue = reader[key];
                                    var TypeAndSize = ColumnTypeAndSize[key.ToUpper()];
                                    whereClauseFields[key] = new Tuple<Type, string>(TypeAndSize.Item1, fieldValue.ToString());
                                }

                                // Update the row with the encrypted values
                                updateQuery = "UPDATE [" + szTableName + "] SET ";
                                foreach (KeyValuePair<string, string> fldToEnc in encfieldNames)
                                {
                                    if (string.IsNullOrEmpty(fldToEnc.Value))
                                        continue;

                                    var TypeAndSize = ColumnTypeAndSize[fldToEnc.Key.ToUpper()];
                                    updateQuery += " [" + fldToEnc.Key + "] =" + FormatField(TypeAndSize.Item1, fldToEnc.Value) + ",";
                                }
                                updateQuery = updateQuery.TrimEnd(',');
                                updateQuery += " WHERE " + ConstructWhereClause(whereClauseFields, szLstFilterOperators);
                                /*using (OdbcCommand updateCommand = new OdbcCommand(updateQuery, connection))
                                {
                                    updateCommand.ExecuteNonQuery();
                                }*/

                                //if(!updateQueries.Contains(updateQuery))
                                updateQueries.Add(updateQuery);

                                if (rows_updated % 1000 == 0)
                                    logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptDBTable: {rows_updated} rows added.");
                            }

                            reader.Close();

                            logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptDBTable: {updateQueries.Count} in list.");
                            List<string> distinctQueries = updateQueries.Distinct().ToList();
                            logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptDBTable: {distinctQueries.Count} distinct queries.");

                            UpdateDBTable(distinctQueries, connection);

                            connection.Close();
                            logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptDBTable: {distinctQueries.Count} update queries executed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions here
                    LastError = ex.Message;
                    logger.LogMessage(Logger.LogLevel.Error, $"BulkDecryptDBTable: {ex.Message} : UpdateQuery: {updateQuery}");
                    connection.Close();
                    return -1;
                }

            }

            return 0;
        }

        //Construct the where clause from the where clause fields
        //format:  Dictionary <string fldName, Tuple<Type fldType, string fldValue>> whereClauseFields
        //The Tuple contains the field type and field value And the szLstFilterOperators contains comma separated operators for the where clause
        virtual public string ConstructWhereClause(Dictionary<string, Tuple<Type, string>> whereClauseFields, string szLstFilterOperators)
        {
            throw new NotImplementedException();
        }

        virtual public string FormatField(Type fldType, string szFieldValue)
        {
            throw new NotImplementedException();
        }

        virtual public int UpdateDBTable(List<string> lstQueries, OdbcConnection conn)
        {
            throw new NotImplementedException();
        }
    }
}
