using Serilog.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.Data.OleDb;
using System.Data.Common;
using System.Collections.Concurrent;

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
        public bool useDistinct = false;

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

                    if(useDistinct)
                    {
                        selectQuery = "SELECT DISTINCT ";
                        foreach (var encfield in encfieldNames)
                        {
                            selectQuery += $"[{encfield.Key}],";
                        }
                        selectQuery = selectQuery.TrimEnd(',');
                        foreach (var whereClauseField in whereClauseFields)
                        {
                            if(!encfieldNames.ContainsKey(whereClauseField.Key))
                                selectQuery += $",[{whereClauseField.Key}]";
                        }
                        selectQuery += $" FROM [{szTableName}]";
                    }

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
                            connection.Dispose();
                            reader.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    connection.Dispose();
                    // Handle any exceptions here
                    LastError = ex.Message;
                    logger.LogMessage(Logger.LogLevel.Error, $"BulkEncryptDBTable: {ex.Message} : UpdateQuery: {updateQuery}");
                    return -1;
                }

            }

            return 0;
        }

        public string ExecuteNameContainsSearch(string connStr, string selectQuery, string nameContainsFilter, 
                                                string colToReturn, int colType)
        {
            logger.LogMessage(Logger.LogLevel.Debug, $"ExecuteNameContainsSearch, SQL: {selectQuery}, filter: {nameContainsFilter}");

            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader reader = null;
            int TableSize = 200;

            if (connStr.ToUpper().StartsWith("PROVIDER"))
            {
                conn = new OleDbConnection(connStr);
                cmd = new OleDbCommand(selectQuery, (OleDbConnection)conn);
            }
            else
            {
                conn = new OdbcConnection(connStr);
                cmd = new OdbcCommand(selectQuery, (OdbcConnection)conn);
            }
            


            string szResult = "";
            DataTable dt = new DataTable();

            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();

                DataTable schemaTable = new DataTable();
                // Create columns in DataTable based on the schema of the reader
                for (int i = 0; i < reader.FieldCount; i++)
                    schemaTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));

                dt = schemaTable.Clone();
                List<DataTable> tblList = new List<DataTable>();

                // Loop through the rows
                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                        row[i] = reader[i];

                    dt.Rows.Add(row);

                    if(dt.Rows.Count >= TableSize)
                    {
                        tblList.Add(dt);
                        dt = new DataTable();
                        dt = schemaTable.Clone();
                    }
                }


                // Add the last table to the list
                if (dt.Rows.Count > 0)
                    tblList.Add(dt);

                dt.Dispose();

                reader.Close();
                conn.Close();
                cmd = null;
                conn = null;
                reader = null;

                if (tblList.Count == 0)
                {
                    logger.LogMessage(Logger.LogLevel.Info, $"ExecuteNameContainsSearch: there no records in the table. SQL: {selectQuery}");
                    return "";
                }


                //Loop through the rows and cols to first decrypt information in each column of the row
                /*foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (row[col] != null && row[col] != DBNull.Value && row[col].ToString().StartsWith("_NDP_"))
                        {
                            string szDecrypted = encryptor.DecryptTextAES(row[col].ToString());
                            row[col] = szDecrypted;
                        }
                    }
                }*/

                logger.LogMessage(Logger.LogLevel.Debug, $"ExecuteNameContainsSearch: {tblList.Count} threads, processing {TableSize} rows each.");

                // Create tasks to process smaller DataTables in parallel
                Task[] tasks = new Task[tblList.Count];

                for (int i = 0; i < tblList.Count; i++)
                {
                    DataTable smallerDt = tblList[i];

                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        ProcessDataTable(smallerDt, encryptor);
                    });
                }

                // Wait for all tasks to complete
                Task.WaitAll(tasks);

                DataTable dataTable = schemaTable.Clone();

                foreach (DataTable tbl in tblList)
                {
                    dataTable.Merge(tbl);
                }

                logger.LogMessage(Logger.LogLevel.Debug, $"ExecuteNameContainsSearch: {dataTable.Rows.Count} rows decrypted.");

                DataRow[] filteredRows = dataTable.Select(nameContainsFilter);

                logger.LogMessage(Logger.LogLevel.Debug, $"ExecuteNameContainsSearch: {filteredRows.Length} rows returned after filter.");

                if (filteredRows.Length == 0)
                {
                    logger.LogMessage(Logger.LogLevel.Info, $"ExecuteNameContainsSearch: No records returned. Filter: {nameContainsFilter}");
                    return "";
                }

                string addQuotes = "";

                if (colType == 2)
                    addQuotes = "'";

                //return all the values in the column ColsToReturn in a comma separated string format
                foreach (DataRow row in filteredRows)
                {
                    if (szResult != "")
                        szResult += ",";

                    if (row[colToReturn] != null && row[colToReturn] != DBNull.Value)
                        szResult += addQuotes + row[colToReturn].ToString() + addQuotes;
                }

            }
            catch (Exception ex)
            {
                logger.LogMessage(Logger.LogLevel.Error, $"ExecuteNameContainsSearch: {ex.Message}");
                return "";
            }

            return szResult;
        }

        private List<DataTable> DivideDataTable(DataTable originalDataTable, int batchSize)
        {
            List<DataTable> smallerDataTables = new List<DataTable>();

            for (int i = 0; i < originalDataTable.Rows.Count; i += batchSize)
            {
                int end = Math.Min(i + batchSize, originalDataTable.Rows.Count);
                DataTable smallerDt = originalDataTable.Clone(); // Clone the schema

                for (int j = i; j < end; j++)
                {
                    DataRow row = originalDataTable.Rows[j];
                    smallerDt.ImportRow(row);
                }

                smallerDataTables.Add(smallerDt);
            }

            return smallerDataTables;
        }

        // Process a DataTable by decrypting rows
        static void ProcessDataTable(DataTable dt, Encryptor encryptor)
        {
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (row[col] != null && row[col] != DBNull.Value && row[col].ToString().StartsWith("_NDP_"))
                    {
                        string szDecrypted = encryptor.DecryptTextAES(row[col].ToString());
                        row[col] = szDecrypted;
                    }
                }
            }
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

                    if (useDistinct)
                    {
                        selectQuery = "SELECT DISTINCT ";
                        foreach (var encfield in encfieldNames)
                        {
                            selectQuery += $"[{encfield.Key}],";
                        }
                        selectQuery = selectQuery.TrimEnd(',');
                        foreach (var whereClauseField in whereClauseFields)
                        {
                            if (!encfieldNames.ContainsKey(whereClauseField.Key))
                                selectQuery += $",[{whereClauseField.Key}]";
                        }
                        selectQuery += $" FROM [{szTableName}]";
                    }

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
                            connection.Dispose();
                            reader.Dispose();

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
                    connection.Dispose();
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
