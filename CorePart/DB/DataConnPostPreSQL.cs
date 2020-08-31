using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Npgsql;
using System.Data;
using System.Collections.Specialized;

namespace iTotal.CorePart
{
    public class DataConnPostpreSQL : DataConnCore
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private NpgsqlConnection _conn = new NpgsqlConnection();
        private NpgsqlTransaction _trans;
        private NpgsqlCommand _command = new NpgsqlCommand();

        public NpgsqlTransaction Trans
        {
            get { return _trans; }
            set { _trans = value; }
        }
        public NpgsqlConnection conn
        {
            get { return _conn; }
            set { _conn = value; }
        }
        public NpgsqlCommand DBCommand
        {
            get { return _command; }
            set { _command = value; }
        }

        public DataConnPostpreSQL()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        
        #region SQL Related Function
        public string getValue(String module, String Table, String Column)
        {
            return getValue(module, Table, Column, null, null);
        }

        public string getValue(String module, String Table, String Column, String Code, String criteriaValue)
        {
            NpgsqlConnection conn = new NpgsqlConnection(getConnStr(module));
            conn.Open();
            String sql;
            string txtID = "";
            sql = "select " + Column + " from " + Table;
            if (criteriaValue != null) { sql = sql + " where " + Code + " = @code"; }

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            if (criteriaValue != null) { cmd.Parameters.Add("@code", NpgsqlTypes.NpgsqlDbType.Varchar, 50).Value = criteriaValue; }

            try
            {
                txtID = (string)cmd.ExecuteScalar();
            }
            catch (NpgsqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + sql + "]");
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            return txtID;
        }

        public string getValue(String module, String sql)
        {
            NpgsqlConnection conn = new NpgsqlConnection(getConnStr(module));
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            return getValue(cmd);
        }

        public string getValue(NpgsqlCommand cmd)
        {
            NpgsqlConnection conn = cmd.Connection;
            string txtID = "";

            try
            {
                if (_trans != null) { cmd.Transaction = _trans; }
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                txtID = (String)cmd.ExecuteScalar();
            }
            catch (NpgsqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
            }
            finally
            {
                if (_trans == null)
                {
                    conn.Close();
                }
            }
            return txtID;

        }

        public int RunSQL(NpgsqlCommand cmd)
        {
            int result = 0;
            NpgsqlTransaction lTran;
            try
            {
                cmd.Connection = _conn;
                if (_trans != null)
                {
                    cmd.Transaction = _trans;
                }
                //Add by Kenneth Tang 2008-10-26 for make sure all update should have begin tran and commit
                else
                {
                    if (_conn.State != ConnectionState.Open) { _conn.Open(); }
                    lTran = cmd.Connection.BeginTransaction();
                    cmd.Transaction = lTran;
                }
                if (_conn.State != ConnectionState.Open) { _conn.Open(); }
                result = cmd.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
            }
            catch (Exception e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
            }
            finally
            {
                if (_trans == null)
                {
                    cmd.Transaction.Commit();
                    _conn.Close();
                }
            }
            return result;
        }

        public DataTable GetDataTable(NpgsqlCommand cmd)
        {
            if (_trans != null) { cmd.Transaction = _trans; }
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            try
            {
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                da.Fill(ds, "dt");
                dt = ds.Tables["dt"];
            }
            catch (NpgsqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
            }
            finally
            {
                if (_trans == null)
                {
                    conn.Close();
                }
            }
            return dt;

        }


        public DataTable GetDataTable(String SQL, NpgsqlConnection connection)
        {
            if (_conn.ConnectionString == "") { _conn = connection; }
            NpgsqlCommand cmd = new NpgsqlCommand(SQL, _conn);
            if (_trans != null) { cmd.Transaction = _trans; }
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                if (_conn.State != ConnectionState.Open) { _conn.Open(); }
                da.Fill(ds, "dt");
                dt = ds.Tables["dt"];
            }
            catch (NpgsqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + SQL + "]");
            }
            finally
            {
                if (_conn != null)
                {
                    if (_trans == null)
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public String getMax(String module, String TableName, String ColumnName)
        {
            // 1. Instantiate the connection
            NpgsqlConnection conn = new NpgsqlConnection(getConnStr(module));
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("Select Max(" + ColumnName + ") MaxVal from " + TableName, conn);
            DataSet ds = new DataSet();
            String s = "";

            try
            {
                conn.Open();
                da.Fill(ds, "MaxVal");
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            foreach (DataRow row in ds.Tables["MaxVal"].Rows)
            {
                s = row["MaxVal"].ToString();
            }
            return s;
        }

        public String getMax(String module, String TableName, String ColumnName, String Criteria)
        {
            // 1. Instantiate the connection
            NpgsqlConnection conn = new NpgsqlConnection(getConnStr(module));
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("Select Max(" + ColumnName + ") MaxVal from " + TableName + " where " + Criteria, conn);
            DataSet ds = new DataSet();
            String s = "";

            try
            {
                conn.Open();
                da.Fill(ds, "MaxVal");
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            foreach (DataRow row in ds.Tables["MaxVal"].Rows)
            {
                s = row["MaxVal"].ToString();
            }
            return s;
        }

        #endregion
 
  
    }
}
