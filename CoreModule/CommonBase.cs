using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iTotal.CorePart;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace iTotal.CoreModule
{
    public class CommonBase : CoreClass
    {

        public CommonBase()
        {
        }

        #region Basic Config DB Function
        protected static string GetConnectionString()
        {
            return GetConnectionString("DBConnectionString");
        }

        protected static string GetConnectionString(String Module)
        {
            return ConfigurationManager.AppSettings[Module];
        }


        protected static void OpenConnection(SqlConnection connection)
        {
            connection.ConnectionString = GetConnectionString();
            connection.Open();
        }

        protected static void OpenConnection(SqlConnection connection, String Module)
        {
            connection.ConnectionString = GetConnectionString(Module);
            connection.Open();
        }

        public static String GetConfigValue(String ConfigType)
        {

            DataTable file = new DataTable();
            String ConfigValue = "";
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                String lsSql = "select ConfigValue from defconfig where ConfigType = '" + ConfigType + "'";
                cmd.CommandText = lsSql;
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                adapter.SelectCommand = cmd;
                adapter.Fill(file);

                connection.Close();
            }
            foreach (DataRow row in file.Rows)
            {
                ConfigValue = row["ConfigValue"].ToString();
            }

            return ConfigValue;
        }

        public static String GetConfigValue(String Module, String ConfigType)
        {

            DataTable file = new DataTable();
            String ConfigValue = "";
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection, Module);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                String lsSql = "select ConfigValue from sysConfigIndicator where ConfigType = '" + ConfigType + "'";
                cmd.CommandText = lsSql;
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                adapter.SelectCommand = cmd;
                adapter.Fill(file);

                connection.Close();
            }
            foreach (DataRow row in file.Rows)
            {
                ConfigValue = row["ConfigValue"].ToString();
            }

            return ConfigValue;
        }
        #endregion

        protected String getSQLHeader()
        {
            String lsSql = "OPEN SYMMETRIC KEY SymmetricKeyHRM";
            lsSql += " DECRYPTION BY CERTIFICATE CertificateHRM;";

            return lsSql;
        }

        protected String getSQLFooter()
        {
            return " CLOSE SYMMETRIC KEY SymmetricKeyHRM;";
        }
    }
}
