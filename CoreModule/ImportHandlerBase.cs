using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iTotal.CoreModule;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Data.OleDb;

namespace iTotal.CoreModule
{
    public class ImportHandlerBase : CommonClass
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ImportHandlerBase()
        {
        }

        protected String CheckTable(String TableName, String ColumnCode, String ColumnDesc, String DescValue, String ExtraCriteria)
        {
            sql = "Select isnull(" + ColumnCode + ",'') from " + TableName + " where " + ColumnDesc + " = '" + DescValue + "' " + ExtraCriteria;
            cmd = new SqlCommand(sql, dc.conn);
            return dc.getValue(cmd);
        }

        protected DataSet ExcelParse(string fileName)
        {
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;  data source={0};Extended Properties=Excel 12.0;", fileName);


            DataSet data = new DataSet();

            foreach (var sheetName in GetExcelSheetNames(connectionString))
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    var dataTable = new DataTable();
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    dataTable.TableName = sheetName;
                    data.Tables.Add(dataTable);
                }
            }

            return data;
        }

        protected string[] GetExcelSheetNames(string connectionString)
        {
            OleDbConnection con = null;
            DataTable dt = null;
            con = new OleDbConnection(connectionString);
            con.Open();
            dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                log.Debug("Record Debug +  error - dt is null - " + con.ConnectionString);
                return null;
            }

            String[] excelSheetNames = new String[dt.Rows.Count];
            int i = 0;

            foreach (DataRow row in dt.Rows)
            {
                log.Debug("Record Debug " +  row["TABLE_NAME"].ToString());
                excelSheetNames[i] = row["TABLE_NAME"].ToString();
                i++;
            }
            con.Close();
            return excelSheetNames;
        }
    }
}
