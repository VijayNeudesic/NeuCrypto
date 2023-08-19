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
        protected Logger logger = new Logger();
        protected Encryptor encryptor = new Encryptor();
        public int row_count = 0;
        public int rows_updated = 0;
        protected string connString = "";

        public EncryptDB(Logger logger, Encryptor _enc)
        {
            this.logger = logger;
            this.encryptor = _enc;
        }

        virtual public int BulkEncryptDBTable(string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            throw new NotImplementedException();
        }

        virtual public int BulkDecryptDBTable(string szTableName, string szFieldNames, string szWhereClauseFields, string szLstFilterOperators)
        {
            throw new NotImplementedException();
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

    }
}
