using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Collections.Specialized;

namespace iTotal.CorePart
{
    public class DataConnCore
    {

        protected String _ConnectUser;
        protected String _ConnectPassword;
        protected String _ConnectServer;
        protected String _ConnectDB;
        protected String _Factor = "KennethTangAnnieKan";

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public String ConnectUser
        {
            get { return _ConnectUser; }
        }

        public String ConnectPassword
        {
            get { return _ConnectPassword; }
        }
        public String ConnectServer
        {
            get { return _ConnectServer; }
        }
        public String ConnectDB
        {
            get { return _ConnectDB; }
        }

        public String GetSetting(String section, String name, String default_value)
        {

            NameValueCollection name_values
                = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(section);
            if (string.IsNullOrEmpty(name_values[name]))
            {
                return default_value;
            }
            else
            {
                return name_values[name];
            }
        }

        #region Get Connecting String
        public string getConnStr(String module)
        {
            int nLeftP;
            int nRightP;
            int startIndex;
            int length;
            String str = ConfigurationManager.ConnectionStrings[module].ConnectionString;

            nLeftP = str.IndexOf("Data Source=");
            nRightP = str.IndexOf(";Initial Catalog");
            startIndex = nLeftP + 12;
            length = nRightP - startIndex;
            _ConnectServer = str.Substring(startIndex, length);
            nLeftP = str.IndexOf("Initial Catalog=");
            nRightP = str.IndexOf(";User ID");
            startIndex = nLeftP + 16;
            length = nRightP - startIndex;
            if (length == 0)
            {
                String sDB = "";
                _ConnectDB = GetSetting(ConfigurationManager.AppSettings["CurrClient"], module, "");
                str = str.Replace("Initial Catalog=" + sDB, "Initial Catalog=" + _ConnectDB);
            }
            else
                _ConnectDB = str.Substring(startIndex, length);

            nLeftP = str.IndexOf("User ID=");
            nRightP = str.IndexOf(";Password");
            startIndex = nLeftP + 8;
            length = nRightP - startIndex;
            String userID = str.Substring(startIndex, length);
            _ConnectUser = deCode(userID, true, "D", 7, "HeaterBee", true);
            nLeftP = str.IndexOf("Password=");
            nRightP = str.Length;
            startIndex = nLeftP + 9;
            length = nRightP - startIndex;
            String pass = str.Substring(startIndex, length);
            _ConnectPassword = deCode(pass, true, "D", 7, "HeaterBee", true);
            str = str.Replace("User ID=" + userID, "User ID=" + _ConnectUser);
            str = str.Replace("Password=" + pass, "Password=" + _ConnectPassword);
            return str;
        }
        #endregion

        #region Decrypt/Encrypt
        public string reverseStr(String Msg)
        {
            char[] strArray = Msg.ToCharArray();
            Array.Reverse(strArray);
            return new string(strArray);
        }

        public string deCode(String lvValue)
        {
            return deCode(lvValue, true);
        }
        public string deCode(String lvValue, Boolean lbReverse)
        {
            return deCode(lvValue, lbReverse, "E");
        }
        public string deCode(String lvValue, Boolean lbReverse, String lsMode)
        {
            return deCode(lvValue, lbReverse, lsMode, 13);
        }
        public string deCode(String lvValue, Boolean lbReverse, String lsMode, int liFactor)
        {
            return deCode(lvValue, lbReverse, lsMode, liFactor, _Factor, false);
        }

        public string deCode(String lvValue, Boolean lbReverse, String lsMode, int liFactor, String lsAppname, Boolean lbForceEncrypt)
        {
            string lsRslt = "";
            byte[] ByteArray;
            //long lllncr;

            if (string.IsNullOrEmpty(lvValue) || (GetSetting(ConfigurationManager.AppSettings["CurrClient"], "Encrypt", "TRUE") == "FALSE" && !lbForceEncrypt))
            {
                return lvValue;
            }
            //switch (lsAppname.Length + liFactor % 6)
            //    {
            //    case 0:
            //        lllncr = 3;
            //        break;
            //    case 1:
            //        lllncr = 5;
            //        break;
            //    case 2:
            //        lllncr = 11;
            //        break;
            //    case 3:
            //        lllncr = 9;
            //        break;
            //    case 4:
            //        lllncr = -2;
            //        break;
            //    case 5:
            //        lllncr = -3;
            //        break;
            //    case 6:
            //        lllncr = -1;
            //        break;
            //    default:
            //        lllncr = 7;
            //        break;
            //    }
            //switch (lsMode)
            //{
            //    case "D":
            //        if (lbReverse) {lvValue= reverseStr(lvValue);}
            //        lvValue = lvValue.Substring(0,1) + lvValue.Substring(2);
            //        for (int i = 0; i < lvValue.Length; i++)
            //        {
            //            ByteArray = System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1));
            //            lsRslt = lsRslt + Convert.ToChar(System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1))[0] + (i+1) - lllncr);
            //        }
            //        break;
            //    case "E":
            //        for (int i = 0; i < lvValue.Length; i++)
            //        {
            //            ByteArray = System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1));
            //            lsRslt = lsRslt + Convert.ToChar(System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1))[0] - (i+1) + lllncr);
            //        }
            //        lsRslt = lsRslt.Substring(0, 1) + Convert.ToChar(System.Text.Encoding.GetEncoding(1251).GetBytes(lsAppname.Substring(0, 1))[0] + lsAppname.Length) + lsRslt.Substring(1); 
            //    //+ (Convert.ToInt32(lsAppname.Substring(1, 1)) + lsAppname.Length).ToString + lsRslt.Substring(1, 2);
            //        if (lbReverse) {lsRslt= reverseStr(lsRslt);}
            //        break;
            //    default:
            //        lsRslt= lvValue;
            //        break;
            //}
            switch (lsMode)
            {
                case "D":
                    //                if (lbReverse) { lvValue = reverseStr(lvValue); }
                    //lvValue = lvValue.Substring(0, 1) + lvValue.Substring(2);
                    for (int i = 0; i < lvValue.Length; i++)
                    {
                        String tmp;
                        tmp = lvValue.Substring(i, 1);
                        ByteArray = System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1));
                        if ((tmp == "~") || (tmp == "^"))
                        {
                            lsRslt = lsRslt + (char)(ByteArray[0] - 4);
                        }
                        else
                        //lsRslt = lsRslt + Convert.ToChar(System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1))[0] +8);
                        {
                            lsRslt = lsRslt + (char)(ByteArray[0] - 1);
                        }
                    }
                    break;
                case "E":
                    for (int i = 0; i < lvValue.Length; i++)
                    {
                        String tmp;
                        tmp = lvValue.Substring(i, 1);
                        ByteArray = System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1));
                        //if (Convert.ToChar(ByteArray[0] - 15) == 34)
                        //{
                        if ((tmp == "Z") || (tmp == "z"))
                        {
                            lsRslt = lsRslt + (char)(ByteArray[0] + 4);
                        }
                        else
                        {
                            lsRslt = lsRslt + (char)(ByteArray[0] + 1);
                        }
                        //}
                        //else
                        //{
                        //    lsRslt = lsRslt + Convert.ToChar(ByteArray[0] - 15);
                        //}
                    }
                    //lsRslt = lsRslt.Substring(0, 1) + Convert.ToChar(System.Text.Encoding.GetEncoding(1251).GetBytes(lsAppname.Substring(0, 1))[0] + lsAppname.Length) + lsRslt.Substring(1);
                    break;
                default:
                    lsRslt = lvValue;
                    break;
            }
            return lsRslt;
        }

        public String deCodeOrg(String lvValue, Boolean lbReverse, String lsMode, int liFactor, String lsAppname)
        {
            string lsRslt = "";
            byte[] ByteArray;
            long lllncr;

            if (string.IsNullOrEmpty(lvValue))
            {
                return lvValue;
            }
            switch (lsAppname.Length + liFactor % 6)
            {
                case 0:
                    lllncr = 3;
                    break;
                case 1:
                    lllncr = 5;
                    break;
                case 2:
                    lllncr = 11;
                    break;
                case 3:
                    lllncr = 9;
                    break;
                case 4:
                    lllncr = -2;
                    break;
                case 5:
                    lllncr = -3;
                    break;
                case 6:
                    lllncr = -1;
                    break;
                default:
                    lllncr = 7;
                    break;
            }
            switch (lsMode)
            {
                case "D":
                    if (lbReverse) { lvValue = reverseStr(lvValue); }
                    lvValue = lvValue.Substring(0, 1) + lvValue.Substring(2);
                    for (int i = 0; i < lvValue.Length; i++)
                    {
                        ByteArray = System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1));
                        lsRslt = lsRslt + Convert.ToChar(System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1))[0] + (i + 1) - lllncr);
                    }
                    break;
                case "E":
                    for (int i = 0; i < lvValue.Length; i++)
                    {
                        ByteArray = System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1));
                        lsRslt = lsRslt + Convert.ToChar(System.Text.Encoding.GetEncoding(1251).GetBytes(lvValue.Substring(i, 1))[0] - (i + 1) + lllncr);
                    }
                    lsRslt = lsRslt.Substring(0, 1) + Convert.ToChar(System.Text.Encoding.GetEncoding(1251).GetBytes(lsAppname.Substring(0, 1))[0] + lsAppname.Length) + lsRslt.Substring(1);
                    //+ (Convert.ToInt32(lsAppname.Substring(1, 1)) + lsAppname.Length).ToString + lsRslt.Substring(1, 2);
                    if (lbReverse) { lsRslt = reverseStr(lsRslt); }
                    break;
                default:
                    lsRslt = lvValue;
                    break;
            }
            return lsRslt;
        }
        #endregion
    }
}
