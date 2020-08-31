using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace iTotal.CoreModule
{
    public class BasePage : System.Web.UI.Page
    {
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

        public static String GetIndicatorValue(String ConfigType)
        {

            DataTable file = new DataTable();
            String ConfigValue = "";
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                String lsSql = "select ConfigValue from sysConfigIndicator where configtype = '" + ConfigType + "'";
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

                String lsSql = "select ConfigValue from defconfig where configtype = '" + ConfigType + "'";
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
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!User.Identity.IsAuthenticated)
            //{
            //    FormsAuthentication.RedirectToLoginPage();
            //}
        }

        public static System.Text.StringBuilder ReadHtmlFile(string htmlFileNameWithPath)
        {
            System.Text.StringBuilder htmlContent = new System.Text.StringBuilder();
            string line;
            try
            {
                using (System.IO.StreamReader htmlReader = new System.IO.StreamReader(htmlFileNameWithPath))
                {

                    while ((line = htmlReader.ReadLine()) != null)
                    {
                        htmlContent.Append(line);
                    }
                }
            }
            catch (Exception objError)
            {
                throw objError;
            }

            return htmlContent;
        }

        protected bool CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                if (dt == DateTime.MinValue) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
