using System;
using System.Data;
using System.Diagnostics;

using System.Data.SqlClient;
/// <summary>
/// Summary description for DataConn
/// </summary>
namespace iTotal.CorePart
{
    public class DataConn : DataConnCore
    {
        private SqlConnection _conn = new SqlConnection();
        private SqlTransaction _trans ;
        private SqlCommand _command = new SqlCommand();
        protected Stopwatch stopWatch = new Stopwatch();

        public SqlTransaction Trans
        {
            get { return _trans; }
            set { _trans = value; }
        }
        public SqlConnection conn
        {
            get { return _conn; }
            set { _conn = value; }
        }
        public SqlCommand DBCommand
        {
            get { return _command; }
            set { _command = value; }
        }

        public DataConn()
        {
            //
            // TODO: Add constructor logic here
            //
        }

#region System Properties Function
        // Get Properties from defSystem
        public DataTable getProperties(String module, String criteria)
        {
            // 1. Instantiate the connection
            SqlConnection conn = new SqlConnection(getConnStr(module));
            DataTable dt = GetDataTable("Select * from defconfig where ConfigType like '" + criteria + "%'", conn);
            conn.Dispose();
            return dt;
        }
        
        
        public DataTable getProperties(String criteria)
        {
            // 1. Instantiate the connection
            SqlConnection conn = new SqlConnection(getConnStr("SET"));
            DataTable dt;
            dt = GetDataTable("Select * from defSystem where Property like '" + criteria + "%'", conn);
            conn.Dispose();
            return dt;
        }

        public int UpdateProperties(String Property, String Code, String ColumnValue)
        {
            int rtn;
            
            SqlConnection conn = new SqlConnection(getConnStr("SET"));
            String sql = "Update defSystem set Value = @ColumnValue where Property = @Property and Code= @Code ";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Property", SqlDbType.NVarChar, 300).Value = Property;
            cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 20).Value = Code;
            cmd.Parameters.Add("@ColumnValue", SqlDbType.NVarChar, 300).Value = ColumnValue;
            rtn = RunSQL(cmd);
            conn.Dispose();
            return rtn;
        }

        public int UpdateProperties(String Property, String ColumnValue)
        {
            int rtn;

            SqlConnection conn = new SqlConnection(getConnStr("SET"));
            String sql = "Update defConfig set ConfigValue = @ColumnValue where ConfigType = @Property ";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Property", SqlDbType.NVarChar, 300).Value = Property;
            cmd.Parameters.Add("@ColumnValue", SqlDbType.NVarChar, 300).Value = ColumnValue;
            rtn = RunSQL(cmd);
            conn.Dispose();
            return rtn;
        }
#endregion


#region SQL Related Function
        public string getValue(String module, String Table, String Column)
        {
            return getValue(module, Table, Column, null, null);
        }

        public int getValueinInt(SqlCommand cmd)
        {
            SqlConnection conn = cmd.Connection;
            int txtID = 0;

            try
            {
                stopWatch.Start();
                log.Debug("SQL Statement : " + cmd.CommandText);

                if (_trans != null) { cmd.Transaction = _trans; }
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                txtID = (int)cmd.ExecuteScalar();
            }
            catch (SqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
            }
            finally
            {
                if (_trans == null)
                {
                    conn.Close();
                }
                stopWatch.Stop();
                log.Debug("SQL processing time : " + stopWatch.Elapsed.TotalSeconds.ToString() + " seconds ");
            }
            return txtID;

        }
        public string getValue(String module, String Table, String Column, String Code, String criteriaValue)
        {
            SqlConnection conn = new SqlConnection(getConnStr(module));
            conn.Open();
            String sql;
            string txtID = "";
            sql = "select " + Column + " from " + Table;
            if (criteriaValue != null) { sql = sql + " where " + Code + " = @code"; }

            SqlCommand cmd = new SqlCommand(sql, conn);

            if (criteriaValue != null) { cmd.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = criteriaValue; }

            try
            {
                txtID = (string)cmd.ExecuteScalar();
            }
            catch (SqlException e)
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
            SqlConnection conn = new SqlConnection(getConnStr(module));
            SqlCommand cmd = new SqlCommand(sql, conn);
            return getValue(cmd);
        }

        public string getValue(SqlCommand cmd)
        {
            SqlConnection conn = cmd.Connection;
            string txtID = "";

            try
            {
                stopWatch.Start();
                log.Debug("SQL Statement : " + cmd.CommandText);

                if (_trans != null) { cmd.Transaction = _trans; }
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                txtID = (String)cmd.ExecuteScalar();
            }
            catch (SqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
            }
            finally
            {
                if (_trans == null)
                {
                    conn.Close();
                }
                stopWatch.Stop();
                log.Debug("SQL processing time : " + stopWatch.Elapsed.TotalSeconds.ToString() + " seconds ");
            }
            return txtID;

        }

        //20200720 added by Jason LI get long ID
        public long getValueinLong(SqlCommand cmd)
        {
            SqlConnection conn = cmd.Connection;
            long txtID = 0;

            try
            {
                stopWatch.Start();
                log.Debug("SQL Statement : " + cmd.CommandText);

                if (_trans != null) { cmd.Transaction = _trans; }
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                txtID = (long)cmd.ExecuteScalar();
            }
            catch (SqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
            }
            finally
            {
                if (_trans == null)
                {
                    conn.Close();
                }
                stopWatch.Stop();
                log.Debug("SQL processing time : " + stopWatch.Elapsed.TotalSeconds.ToString() + " seconds ");
            }
            return txtID;

        }

        public int RunSQL(SqlCommand cmd)
        {
            int result = 0;
            SqlTransaction lTran;
            try
            {
                cmd.Connection = _conn;
                stopWatch.Start();
                log.Debug("SQL Statement : " + cmd.CommandText);
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
                if (_conn.State !=  ConnectionState.Open) { _conn.Open(); }
                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
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
                stopWatch.Stop();
                log.Debug("SQL processing time : " + stopWatch.Elapsed.TotalSeconds.ToString() + " seconds ");
            }
            return result;
        }

        public DataTable GetDataTable(SqlCommand cmd)
        {
            if (_trans != null) { cmd.Transaction = _trans; }
            stopWatch.Start();
            log.Debug("SQL Statement : " + cmd.CommandText);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            try
            {
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                da.Fill(ds, "dt");
                dt = ds.Tables["dt"];
            }
            catch (SqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
            }
            finally
            {
                if (_trans == null)
                {
                    conn.Close();
                }
                stopWatch.Stop();
                log.Debug("SQL processing time : " + stopWatch.Elapsed.TotalSeconds.ToString() + " seconds ");
            }
            return dt;

        }


        public DataTable GetDataTable(String SQL, SqlConnection connection)
        {
            if ( _conn.ConnectionString == "" ){ _conn = connection;}
            SqlCommand cmd = new SqlCommand(SQL,_conn);
            if (_trans != null) { cmd.Transaction = _trans; }
            stopWatch.Start();
            log.Debug("SQL Statement : " + SQL);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                if (_conn.State != ConnectionState.Open) { _conn.Open(); }
                da.Fill(ds, "dt");
                dt = ds.Tables["dt"];
            }
            catch (SqlException e)
            {
                log.Error("SQL Error          " + e.Message + "[ " + cmd.CommandText + "]");
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
                stopWatch.Stop();
                log.Debug("SQL processing time : " + stopWatch.Elapsed.TotalSeconds.ToString() + " seconds ");
            }
            return dt;
        }

        public String getMax(String module,String TableName, String ColumnName)
        {
            // 1. Instantiate the connection

            SqlConnection conn = new SqlConnection(getConnStr(module));
            SqlDataAdapter da = new SqlDataAdapter("Select Max(" + ColumnName + ") MaxVal from " + TableName, conn);
            stopWatch.Start();
            log.Debug("SQL Statement : " + "Select Max(" + ColumnName + ") MaxVal from " + TableName);
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
                stopWatch.Stop();
                log.Debug("SQL processing time : " + stopWatch.Elapsed.TotalSeconds.ToString() + " seconds ");
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
            SqlConnection conn = new SqlConnection(getConnStr(module));
            stopWatch.Start();
            log.Debug("SQL Statement : " + "Select Max(" + ColumnName + ") MaxVal from " + TableName);
            SqlDataAdapter da = new SqlDataAdapter("Select Max(" + ColumnName + ") MaxVal from " + TableName + " where " + Criteria, conn);
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
                stopWatch.Stop();
                log.Debug("SQL processing time : " + stopWatch.Elapsed.TotalSeconds.ToString() + " seconds ");
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
