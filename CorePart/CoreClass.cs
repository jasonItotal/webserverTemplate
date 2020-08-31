using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Data.SqlClient;
using System.Data.SqlTypes;

/// <summary>
/// Summary description for CoreClass
/// </summary>
namespace iTotal.CorePart
{
    public class CoreClass
    {
        private DataConn _dc;
        protected SqlConnection conn = new SqlConnection();

        protected String sql;
        protected DataTable dt;
        protected SqlCommand cmd;


        protected static String color_Holiday = "#ffffcc";
        protected static String color_blank_even = "#dbdbb8";
        protected static String color_selected = "#999933";
        protected static String color_background = "#dddddd";
        protected static String color_Sunday = "#f0f0df";
        protected static String color_work_fore = "#a4a516";
        protected static String color_inValid = "#8ae6f7";
        protected static String color_onLeaveFore = "#696912";

        
        public DataConn dc
        {
            get { return _dc; }
            set { _dc = value; }
        }


        public CoreClass()
        {
            //
            // TODO: Add constructor logic here
            //
            _dc = new DataConn();
        }


        #region Create Cell
        protected static TableCell CreateCell(String Content)
        {
            return CreateCell(Content, 0);
        }

        protected static TableCell CreateCell(String Content, Int16 RowSpan)
        {
            return CreateCell(Content, RowSpan, 0);
        }
        protected static TableCell CreateCell(String Content, Int16 RowSpan, Int16 ColSpan)
        {
            return CreateCell(Content, RowSpan, ColSpan, "");
        }

        protected static TableCell CreateCell(String Content, Int16 RowSpan, Int16 ColSpan, String BackColor)
        {
            TableCell cell = new TableCell();
            cell.Text = Content;
            cell.RowSpan = RowSpan;
            cell.ColumnSpan = ColSpan;
            if (BackColor.Length > 0) cell.Style.Add("background - color", BackColor);
            return cell;
        }
        #endregion
        
        
    }

    public class Eval
    {
        // simulate basic evaluation of arithmetic expression 

        public Eval()
        {

        }

        public static bool VerifyAllowed(string e)
        {
            string allowed = "0123456789+-*/().,";
            for (int i = 0; i < e.Length; i++)
            {
                if (allowed.IndexOf("" + e[i]) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        public string Evaluate(string e)
        {
            if (e.Length == 0)
            {
                return "String length is zero";
            }
            if (!VerifyAllowed(e))
            {
                return "The string contains not allowed characters";
            }
            if (e[0] == '-')
            {
                e = "0" + e;
            }
            string res = "";
            try
            {
                res = Calculate(e).ToString();
            }
            catch
            {
                return "The call caused an exception";
            }
            return res;
        }

        public static double Calculate(string e)
        {
            ////e=e.Replace(".",","); 
            if (e.IndexOf("(") != -1)
            {
                int a = e.LastIndexOf("(");
                int b = e.IndexOf(")", a);
                double middle = Calculate(e.Substring(a + 1, b - a - 1));
                return Calculate(e.Substring(0, a) + middle.ToString() + e.Substring(b + 1));
            }
            double result = 0;
            string[] plus = e.Split('+');
            if (plus.Length > 1)
            {
                // there were some + 
                result = Calculate(plus[0]);
                for (int i = 1; i < plus.Length; i++)
                {
                    result += Calculate(plus[i]);
                }
                return result;

            }
            else
            {
                // no + 
                string[] minus = plus[0].Split('-');
                if (minus.Length > 1)
                {
                    // there were some - 
                    result = Calculate(minus[0]);
                    for (int i = 1; i < minus.Length; i++)
                    {
                        result -= Calculate(minus[i]);
                    }
                    return result;

                }
                else
                {
                    // no - 
                    string[] mult = minus[0].Split('*');
                    if (mult.Length > 1)
                    {
                        // there were some * 
                        result = Calculate(mult[0]);
                        for (int i = 1; i < mult.Length; i++)
                        {
                            result *= Calculate(mult[i]);
                        }
                        return result;

                    }
                    else
                    {
                        // no * 
                        string[] div = mult[0].Split('/');
                        if (div.Length > 1)
                        {
                            // there were some / 
                            result = Calculate(div[0]);
                            for (int i = 1; i < div.Length; i++)
                            {
                                result /= Calculate(div[i]);
                            }
                            return result;

                        }
                        else
                        {
                            // no / 
                            return double.Parse(e);
                        }
                    }
                }
            }
        }
    } 

}