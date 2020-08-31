using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace iTotal.CoreModule
{
    public class TableSchemaBase : CommonClass
    {
        private String _UID;
        private String _ImportErr;

        public TableSchemaBase() { }

        public String UID
        {
            get { return _UID; }
            set { _UID = value; }
        }
        public String errText
        {
            get { return _ImportErr; }
            set { _ImportErr = value; }
        }
    }

}
