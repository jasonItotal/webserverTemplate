using System;
using System.Collections.Generic;
using System.Text;
using iTotal.CoreModule;
using System.Data.SqlClient;
using System.Data;

namespace Services
{
    public class BaseService : CommonBase
    {

        protected string SQL = string.Empty;
        public BaseService(string connectionString)
        {
            dc.conn = new SqlConnection(connectionString);
        }
    }

    public static class DataRowExtensions
    {
        public static object GetValue(this DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) ? row[column] : null;
        }

        public static string GetString(this DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) ? row[column].ToString() : "";
        }
    }
}
