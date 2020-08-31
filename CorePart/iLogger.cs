using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace iTotal.CorePart
{
    public class iLogger 
    {
        #region "Variables Declarations"
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection();
        SqlTransaction Trans;

        private string targetTable;
        private string _errorStack;
        private string _requestData;
        private string _responseData;
        private string _logSystem;
        private string _IP;
        private string _method;
        private string _OSUserName = System.Environment.UserName;
        private string _msg;
        private TargetAction _action;
        private string _sql;
        private string _connectionString;

        private string _logDeviceID = System.Environment.MachineName;
        private string _OSversion = System.Environment.OSVersion.ToString();
        private double _responseTime;
        private DateTime _starttime;


        public double ResponseTime
        {
            get { return _responseTime; }
            set { _responseTime = value; }
        }
        public string SQL
        {
            get { return _sql; }
            set { _sql = value; }
        }
        public string LogConnectString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public TargetAction LogAction
        {
            get { return _action; }
            set { _action = value; }
        }
        public string OSVersion
        {
            get { return _OSversion; }
            set { _OSversion = value; }
        }
        public string LogMessage
        {
            get { return _msg; }
            set { _msg = value; }
        }
        public string LogTable 
        {
            get {return targetTable;}
            set {targetTable = value;}
        }

        public string DeviceID
        {
            get { return _logDeviceID; }
            set { _logDeviceID = value; }
        }

        public string DeviceIP
        {
            get { return _IP; }
            set { _IP = value; }
        }

        public string LogFunction
        {
            get { return _method; }
            set { _method = value; }
        }

        public string LogError
        {
            get { return _errorStack; }
            set { _errorStack = value; }
        }

        public string DataRequest
        {
            get { return _requestData; }
            set { _requestData = value; }
        }

        public string DataResponse
        {
            get { return _responseData; }
            set { _responseData = value; }
        }

        public string LogSystem
        {
            get { return _logSystem; }
            set { _logSystem = value; }
        }

        //private string _objectVersion;
        //private string _objectCulture;
        //private DateTime _errorDate;
        //private string _db;        //private string _objectVersion;
        //private string _objectCulture;
        //private DateTime _errorDate;
        //private string _db;

        #endregion


        public iLogger()
        { 
        }

        public void Start(string logSystem, string Method, TargetAction Action, string UID)
        {
            _starttime = DateTime.Now;

            _method = Method;
            _OSUserName = UID;
            _logSystem = logSystem;
            _action = Action;
        }


        protected static string GetConnectionString(String Module)
        {
            return ConfigurationManager.AppSettings[Module];
        }

        protected void BaseLogging(string logLevel)
        {
            _responseTime = ( DateTime.Now - _starttime).TotalSeconds;
            using (conn)
            {
                conn = new SqlConnection(GetConnectionString("iLogger")); 

                String lsSql = "insert into " + targetTable + "(";
                lsSql += " logLevel, logSystem, logType, logDeviceID, OSVersion, Method, Message, errorStack, Request, Response, IP, ResponseTime, SQL, logUser";
                lsSql += " ) values (";
                lsSql += " @logLevel, @logSystem, @logType, @logDeviceID, @OSVersion, @Method, @Message, @errorStack, @Request, @Response, @IP, @ResponseTime, @SQL, @logUser";
                lsSql += " )";

                conn.Open();
                Trans = conn.BeginTransaction();

                cmd = new SqlCommand(lsSql, conn);
                cmd.Parameters.Add("logLevel", SqlDbType.NVarChar, 50).Value = logLevel;
                cmd.Parameters.Add("logSystem", SqlDbType.NVarChar, 50).Value = _logSystem == null ? DBNull.Value : (object)_logSystem;
                cmd.Parameters.Add("@logType", SqlDbType.NVarChar, 200).Value = _action.ToString();
                cmd.Parameters.Add("@logDeviceID", SqlDbType.NVarChar, 200).Value = _logDeviceID == null ? DBNull.Value : (object)_logDeviceID;
                cmd.Parameters.Add("@OSVersion", SqlDbType.NVarChar, 100).Value = _OSversion == null ? DBNull.Value : (object)_OSversion;
                cmd.Parameters.Add("@Method", SqlDbType.NVarChar, 500).Value = _method == null ? DBNull.Value : (object)_method;
                cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 4000).Value = _msg== null ? DBNull.Value : (object)_msg;
                cmd.Parameters.Add("@errorStack", SqlDbType.VarChar, 4000).Value = _errorStack == null ? DBNull.Value : (object)_errorStack;
                cmd.Parameters.Add("@Request", SqlDbType.VarChar, 4000).Value = _requestData == null ? DBNull.Value : (object)_requestData;
                cmd.Parameters.Add("@Response", SqlDbType.VarChar, 4000).Value = _responseData == null ? DBNull.Value : (object)_responseData;
                cmd.Parameters.Add("@SQL", SqlDbType.VarChar, 4000).Value = _sql == null ? DBNull.Value : (object)_sql;
                cmd.Parameters.Add("@IP", SqlDbType.VarChar, 50).Value = _IP == null ? DBNull.Value : (object)_IP;
                cmd.Parameters.Add("@logUser", SqlDbType.VarChar, 50).Value = _OSUserName == null ? DBNull.Value : (object)_OSUserName;
                cmd.Parameters.Add("@ResponseTime", SqlDbType.Decimal).Value = _responseTime;

                try
                {
                    cmd.Transaction = Trans;
                    cmd.ExecuteNonQuery();
                    Trans.Commit();
                }
                catch ( SqlException ex )
                {
                    SaveToFile(ex.Message, "");
                    Trans.Rollback();
                }
                finally
                {
                    conn.Close();
                    cmd.Dispose();
                }
            }
        }

        public void SaveToFile(string SQLError, string TargetFile)
        {
            if (TargetFile.Length == 0 )
                TargetFile = ConfigurationManager.AppSettings["ErrorFilePath"] + "\\PPIS_" + _logDeviceID + "_" +DateTime.Now.ToString("yyyyMMdd") + ".log";
            else
               if (TargetFile.IndexOf(":\\") < 0)
                    TargetFile = ConfigurationManager.AppSettings["ErrorFilePath"] + "\\" + TargetFile;
               
            System.IO.StreamWriter OutputFile = new System.IO.StreamWriter(TargetFile, true);
            string sOutput = "";

            sOutput = "===========================================" + System.Environment.NewLine;
            if (SQLError.Length > 0)
            {
                sOutput += "Logging Error     : " + SQLError + System.Environment.NewLine;
                sOutput += "===========================================" + System.Environment.NewLine;
            }
            sOutput += "DateTime     : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + System.Environment.NewLine;
            sOutput += "Machine Name : " + DeviceID + System.Environment.NewLine;
            sOutput += "OS Version   : " + OSVersion + System.Environment.NewLine;
            sOutput += "OS User      : " + _OSUserName + System.Environment.NewLine;
            sOutput += "SQL      : " + SQL +System.Environment.NewLine;
            sOutput += "Error Stack  : " + _errorStack + System.Environment.NewLine;
            sOutput += "===========================================" + System.Environment.NewLine;

            OutputFile.WriteLine(sOutput);
            OutputFile.Close();
        }

        public void Trace()
        {
            BaseLogging("Trace");
        }

        public void Debug()
        {
            BaseLogging("Debug");
        }

        public void Info()
        {
            BaseLogging("Info");
        }

        public void Warning()
        {
            BaseLogging("Warning");
        }

        public void Fatal()
        {
            BaseLogging("Fatal");
        }
    }

    public enum TargetAction
    {
        ADD = 1,
        CHANGE = 2,
        DELETE = 3,
        RETRIEVE = 4,
        GET = 5,
        POST = 6,
        REQUEST = 7,
        RESPONSE = 8
    }

    public enum TargetLog
    {
        MSSQL,
        TextFile
    }

    #region FileLogger
    /// <summary>
    /// 基于文件的日记记录。
    /// </summary>
    public class FileLogger
    {
        private string path;

        #region consturct
        public FileLogger(string fileName)
        {
            path = fileName;
        }
        #endregion

        protected void WriteLog(string msg, string level)
        {
            try
            {
                File.AppendAllText(path, string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString(), level, msg + Environment.NewLine), Encoding.UTF8);
            }
            catch { }
        }
    }
    #endregion
}
