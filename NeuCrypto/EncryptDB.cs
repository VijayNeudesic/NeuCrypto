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
    internal class EncryptDB
    {
        public string LastError { get; set; }
        private Logger logger = new Logger();
        private Encryptor encryptor = new Encryptor();
        public int row_count = 0;
        public int rows_updated = 0;

        public EncryptDB(Logger logger, Encryptor _enc)
        {
            this.logger = logger;
            this.encryptor = _enc;
        }

        public int BulkEncryptAccessDBTable(string szDBPath, string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            // Create the connection string for the Access database
            string connectionString = $"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={szDBPath};";

            logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptAccessDBTable: {szDBPath}, {szTableName}, {szFieldNames}, {szWhereClauseFields}, {szLstFilterOperators}");

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

            // Create a new OleDbConnection using the connection string
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                try
                {
                    // Open the database connection
                    connection.Open();

                    // Create a command to read the data
                    string selectQuery = "SELECT * FROM " + szTableName;

                    logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptAccessDBTable: {selectQuery}");

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
                                        LastError = $"BulkEncryptAccessDBTable: Encrypted value is larger than the column size. Column: {key}, Encrypted value: {encryptedValue}, Column size: {typeAndSize.Item2}";
                                        logger.LogMessage(Logger.LogLevel.Error, LastError);
                                        return -1;
                                    }
                                }
                            }

                            reader.Close();

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
                                    row_count++;

                                    //Had to use the classic for loop to avoid the exception of collection been modified
                                    for (int i = 0; i < encfieldNames.Count; i++)
                                    {
                                        string key = encfieldNames.ElementAt(i).Key;
                                        object fieldValue = reader[key];

                                        // Encrypt the field value if it is not null
                                        if (fieldValue != DBNull.Value && !fieldValue.ToString().StartsWith(Encryptor.EncryptDataHeader))
                                        {
                                            bPerformUpdate = true;
                                            string encryptedValue = encryptor.EncryptTextAES(fieldValue.ToString());
                                            encfieldNames[key] = encryptedValue;
                                        }
                                    }

                                    // If there is no fields that were encrypted, then continue
                                    if (!bPerformUpdate)
                                        continue;

                                    rows_updated++;

                                    // Loop through the where clause fields
                                    for (int i = 0; i < whereClauseFields.Count; i++)
                                    {
                                        string key = whereClauseFields.ElementAt(i).Key;
                                        // Get the value of the field
                                        object fieldValue = reader[key];
                                        var TypeAndSize = ColumnTypeAndSize[key];
                                        whereClauseFields[key] = new Tuple<Type, string>(TypeAndSize.Item1, fieldValue.ToString());
                                    }

                                    // Update the row with the encrypted values
                                    updateQuery = "UPDATE " + szTableName + " SET ";
                                    foreach (KeyValuePair<string, string> fldToEnc in encfieldNames)
                                    {
                                        var TypeAndSize = ColumnTypeAndSize[fldToEnc.Key.ToUpper()];
                                        updateQuery += fldToEnc.Key + "=" + AccessSQLFormatField(TypeAndSize.Item1, fldToEnc.Value) + ",";
                                    }
                                    updateQuery = updateQuery.TrimEnd(',');
                                    updateQuery += " WHERE " + ConstructWhereClause(whereClauseFields, szLstFilterOperators);
                                    using (OdbcCommand updateCommand = new OdbcCommand(updateQuery, connection))
                                    {
                                        updateCommand.ExecuteNonQuery();
                                    }

                                }

                                reader.Close();
                            }
                            else
                            {
                                logger.LogMessage(Logger.LogLevel.Debug, "No rows to encrypt.");
                            }

                            logger.LogMessage(Logger.LogLevel.Debug, $"BulkEncryptAccessDBTable: {rows_updated} rows updated.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions here
                    LastError = ex.Message;
                    logger.LogMessage(Logger.LogLevel.Error, $"BulkEncryptAccessDBTable: {ex.Message} : UpdateQuery: {updateQuery}");
                    return -1;
                }

            }

            return 0;
        }


        public int BulkDecryptAccessDBTable(string szDBPath, string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            // Create the connection string for the Access database
            string connectionString = $"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={szDBPath};";

            logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptAccessDBTable: {szDBPath}, {szTableName}, {szFieldNames}, {szWhereClauseFields}, {szLstFilterOperators}");

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

            // Create a new OleDbConnection using the connection string
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                try
                {
                    // Open the database connection
                    connection.Open();

                    // Create a command to read the data
                    string selectQuery = "SELECT * FROM " + szTableName;

                    logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptAccessDBTable: {selectQuery}");

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
                                row_count++;

                                //Had to use the classic for loop to avoid the exception of collection been modified
                                for (int i = 0; i < encfieldNames.Count; i++)
                                {
                                    string key = encfieldNames.ElementAt(i).Key;
                                    object fieldValue = reader[key];

                                    // Decrypt the field value if it is not null and it starts with the encrypted data header
                                    if (fieldValue != DBNull.Value && fieldValue.ToString().StartsWith(Encryptor.EncryptDataHeader))
                                    {
                                        bPerformUpdate = true;
                                        string encryptedValue = encryptor.DecryptTextAES(fieldValue.ToString());
                                        encfieldNames[key] = encryptedValue;
                                    }
                                }

                                // If there is no fields that were encrypted, then continue
                                if (!bPerformUpdate)
                                    continue;

                                rows_updated++;

                                // Loop through the where clause fields
                                for (int i = 0; i < whereClauseFields.Count; i++)
                                {
                                    string key = whereClauseFields.ElementAt(i).Key;
                                    // Get the value of the field
                                    object fieldValue = reader[key];
                                    var TypeAndSize = ColumnTypeAndSize[key];
                                    whereClauseFields[key] = new Tuple<Type, string>(TypeAndSize.Item1, fieldValue.ToString());
                                }

                                // Update the row with the encrypted values
                                updateQuery = "UPDATE " + szTableName + " SET ";
                                foreach (KeyValuePair<string, string> fldToEnc in encfieldNames)
                                {
                                    var TypeAndSize = ColumnTypeAndSize[fldToEnc.Key.ToUpper()];
                                    updateQuery += fldToEnc.Key + "=" + AccessSQLFormatField(TypeAndSize.Item1, fldToEnc.Value) + ",";
                                }
                                updateQuery = updateQuery.TrimEnd(',');
                                updateQuery += " WHERE " + ConstructWhereClause(whereClauseFields, szLstFilterOperators);
                                using (OdbcCommand updateCommand = new OdbcCommand(updateQuery, connection))
                                {
                                    updateCommand.ExecuteNonQuery();
                                }

                            }

                            reader.Close();


                            logger.LogMessage(Logger.LogLevel.Debug, $"BulkDecryptAccessDBTable: {rows_updated} rows updated.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions here
                    LastError = ex.Message;
                    logger.LogMessage(Logger.LogLevel.Error, $"BulkDecryptAccessDBTable: {ex.Message} : UpdateQuery: {updateQuery}");
                    return -1;
                }

            }

            return 0;
        }


        //Construct the where clause from the where clause fields
        //format:  Id|i,name|s,dob|d
        //The Tuple contains the field type and field value And the szLstFilterOperators contains comma separated operators for the where clause
        private string ConstructWhereClause(Dictionary<string, Tuple<Type, string>> whereClauseFields, string szLstFilterOperators)
        {
            string szRet = "";
            string[] lstFilterOperators = szLstFilterOperators.Split(',');
            int i = 0;
            foreach (KeyValuePair<string, Tuple<Type, string>> fldToEnc in whereClauseFields)
            {
                if (szRet != "")
                    szRet += " " + lstFilterOperators[i++] + " ";

                szRet += fldToEnc.Key + "=" + AccessSQLFormatField(fldToEnc.Value.Item1, fldToEnc.Value.Item2);
            }

            return szRet;
        }

        private string AccessSQLFormatField(Type fldType, string szFieldValue)
        {
            string szRet = "";
            switch (fldType.Name)
            {
                case "Int32":
                    szRet = szFieldValue;
                    break;
                case "String":
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
