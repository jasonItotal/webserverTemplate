using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iTotal.CorePart;
using System.Data;
using System.Data.SqlClient;

namespace iTotal.CoreModule
{
    public class CommonClass : CommonBase
    {
        public CommonClass()
        {
            dc.conn = new SqlConnection(GetConnectionString("SYSTEM"));
        }
    }
}
