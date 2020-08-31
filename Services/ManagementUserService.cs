using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Services.Model;

namespace Services
{
    public class ManagementUserService : BaseService
    {
        public ManagementUserService(string connectionString) : base(connectionString)
        {
        
        }

        public List<ApiManagementUser> List(int page, int limit) {
            List<ApiManagementUser> list = new List<ApiManagementUser>();
            DataTable dt;
            SQL  = "SELECT  *";
            SQL += " FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CR_DATE DESC) AS RowNum, ID, Name, PasswordHash,Token, Expiry";
            SQL += "          FROM      [ApiManagementUser]";
            SQL += "          WHERE     DELETED = 0";
            SQL += "        ) AS RowConstrainedResult";
            SQL += " WHERE   RowNum >= @StartIndex";
            SQL += "    AND RowNum <= @EndIndex";
            SQL += " ORDER BY RowNum";
            cmd = new SqlCommand(SQL, dc.conn);
            var StartIndex = 1 + (page-1) * limit;
            var EndIndex = limit + (page - 1) * limit;
            cmd.Parameters.Add("@StartIndex", SqlDbType.Int).Value = StartIndex;
            cmd.Parameters.Add("@EndIndex", SqlDbType.Int).Value = EndIndex;
            dt = dc.GetDataTable(cmd);
            foreach (DataRow dr in dt.Rows) {
                var record = ConvertDrToEntity(dr);
                list.Add(record);
            }
            return list;
        }

        public int Count() {
            SQL  = "SELECT Count(ID) ";
            SQL += " FROM [ApiManagementUser]";
            SQL += " WHERE DELETED = 0";
            cmd = new SqlCommand(SQL, dc.conn);
            int count = dc.getValueinInt(cmd);
            return count;
        }


        public ApiManagementUser Get(string ID = "", string Token = "", string Name = "", string PasswordHash="") {
            DataTable dt;
            List<SqlParameter> paramList = new List<SqlParameter>();
            SQL = " select ID, Name, PasswordHash,Token, Expiry";
            SQL += " from ApiManagementUser ";
            SQL += " where DELETED = 0 ";
            if (!string.IsNullOrEmpty(ID))
            {
                SQL += " and ID = @ID ";
                var param = new SqlParameter("@ID", SqlDbType.BigInt);
                param.Value = Convert.ToInt64(ID);
                paramList.Add(param);
            }
            if (!string.IsNullOrEmpty(Token))
            {
                SQL += " and Token = @Token ";
                var param = new SqlParameter("@Token", SqlDbType.NVarChar, 1000);
                param.Value = Token;
                paramList.Add(param);
            }
            if (!string.IsNullOrEmpty(Name))
            {
                SQL += " and Name = @Name ";
                var param = new SqlParameter("@Name", SqlDbType.NVarChar, 1000);
                param.Value = Name;
                paramList.Add(param);
            }
            if (!string.IsNullOrEmpty(PasswordHash))
            {
                SQL += " and PasswordHash = @PasswordHash";
                var param = new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 1000);
                param.Value = PasswordHash;
                paramList.Add(param);
            }
            cmd = new SqlCommand(SQL, dc.conn);
            foreach (var param in paramList)
            {
                cmd.Parameters.Add(param);
            }
            dt = dc.GetDataTable(cmd);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                var dr = dt.Rows[0];
                var record = ConvertDrToEntity(dr);
                return record;
            }
        }

        public ApiManagementUser ConvertDrToEntity(DataRow dr) {
            DateTime? expiry = null;
            if (dr["Expiry"] != DBNull.Value)
            {
                expiry = Convert.ToDateTime(dr["Expiry"]);
            }
            var record = new ApiManagementUser()
            {
                ID = Convert.ToInt64(dr["ID"]),
                Name = dr["Name"].ToString(),
                PasswordHash = dr["PasswordHash"].ToString(),
                Token = dr["Token"].ToString(),
                Expiry = expiry,
                //for editing use
                password = "",
                confirm = ""
            };
            return record;
        }


        public long Insert(string Name, string PasswordHash, string Token, string Expiry, string CR_USR)
        {
            long result = 0;
            SQL = " Insert into ApiManagementUser";
            SQL += " (Name, PasswordHash,Token, Expiry, CR_USR)";
            SQL += " OUTPUT Inserted.ID values";
            SQL += " (@Name, @PasswordHash,@Token, @Expiry, @CR_USR)";
            cmd = new SqlCommand(SQL, dc.conn);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 1000).Value = Name;
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 1000).Value = PasswordHash;
            cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 1000).Value = Token;
            if (string.IsNullOrEmpty(Expiry))
            {
                cmd.Parameters.Add("@Expiry", SqlDbType.DateTime).Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters.Add("@Expiry", SqlDbType.DateTime).Value = Convert.ToDateTime(Expiry);
            }
            cmd.Parameters.Add("@CR_USR", SqlDbType.NVarChar, 1000).Value = CR_USR;
            result = dc.getValueinLong(cmd);
            return result;
        }

        public ApiManagementUser GenerateToken(string ID,int minutesAlive) {
            var newToken = Guid.NewGuid().ToString();
            var expairy = DateTime.Now.AddMinutes(minutesAlive);
            Update(ID, newToken, expairy.ToString());
            var mUser = Get(ID);
            return mUser;
        }

        public void Update(string ID, string Token = "", string Expiry = "", string Name="", string PasswordHash="",  string UP_USR = "")
        {
            SQL = " update ApiManagementUser set ";
            List<SqlParameter> paramList = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(Name))
            {
                SQL += " Name = @Name,";
                var param = new SqlParameter("@Name", SqlDbType.NVarChar, 1000);
                param.Value = Name;
                paramList.Add(param);
            }
            if (!string.IsNullOrEmpty(PasswordHash))
            {
                SQL += " PasswordHash = @PasswordHash,";
                var param = new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 1000);
                param.Value = PasswordHash;
                paramList.Add(param);
            }
            if (!string.IsNullOrEmpty(Token))
            {
                SQL += " Token = @Token,";
                var param = new SqlParameter("@Token", SqlDbType.NVarChar, 1000);
                if (Token.Equals("Null"))
                {
                    param.Value = DBNull.Value;
                }
                else {
                    param.Value = Token;
                }
                paramList.Add(param);
            }
            if (!string.IsNullOrEmpty(Expiry))
            {
                SQL += " Expiry = @Expiry,";
                var param = new SqlParameter("@Expiry", SqlDbType.DateTime);
                if (Expiry.Equals("Null"))
                {
                    param.Value = DBNull.Value;
                }
                else
                {
                    param.Value = Convert.ToDateTime(Expiry);
                }
                paramList.Add(param);
            }
            if (!string.IsNullOrEmpty(UP_USR))
            {
                SQL += " UP_USR = @UP_USR,";
                var param = new SqlParameter("@UP_USR", SqlDbType.NVarChar, 1000);
                param.Value = UP_USR;
                paramList.Add(param);

                SQL += " UP_DATE = @UP_DATE,";
                var param2 = new SqlParameter("@UP_DATE", SqlDbType.DateTime);
                param.Value = DateTime.Now;
                paramList.Add(param2);
            }
            //remove last comma
            SQL = SQL.Substring(0, SQL.Length - 1);
            SQL += " where ID = @ID";
            SQL += " and deleted = 0";
            cmd = new SqlCommand(SQL, dc.conn);
            foreach (var param in paramList) {
                cmd.Parameters.Add(param);
            }
            cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = Convert.ToInt64(ID);
            dc.RunSQL(cmd);
        }


        public void Delete(string ID, string DEL_USR)
        {
            SQL = " update ApiManagementUser set ";
            SQL += " deleted = 1,";
            SQL += " DEL_USR = @DEL_USR,";
            SQL += " DEL_DATE = @DEL_DATE";
            SQL += " where ID = @ID";
            SQL += " and deleted = 0";
            cmd = new SqlCommand(SQL, dc.conn);
            cmd.Parameters.Add("@DEL_USR", SqlDbType.NVarChar, 1000).Value = DEL_USR;
            cmd.Parameters.Add("@DEL_DATE", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = Convert.ToInt64(ID);
            dc.RunSQL(cmd);
        }
    }
}
