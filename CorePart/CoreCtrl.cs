using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data.SqlClient;
using System.Text.RegularExpressions;



namespace iTotal.CorePart 
{
    public class CoreCtrl : UserControl
    {
        protected DataConn dc = new DataConn();
        protected String CompCode = "";

        public CoreCtrl()
        {
        }

        #region Subscriber Method
        public virtual void ShowMenu(String Option)
        {
            ShowAdd(false);
            ShowEdit(false);
            ShowDelete(false);
            ShowPrint(false);
            ShowSearch(false);
            
            if (Option.IndexOf("ADD") >= 0) ShowAdd(true);
            if (Option.IndexOf("EDIT") >= 0) ShowEdit(true);
            if (Option.IndexOf("DELETE") >= 0) ShowDelete(true);
            if (Option.IndexOf("PRINT") >= 0) ShowPrint(true);
            if (Option.IndexOf("SEARCH") >= 0) ShowSearch(true);
        }

        #endregion

        #region Access Control
        public Boolean GetAccessRight(String type)
        {
            if (Context.User.Identity.Name != "")
            {
                String module = dc.getValue("iTotal", "defUser", "usr_db", "usr_id", Context.User.Identity.Name);
                String UID = Context.User.Identity.Name;
                CompCode = dc.GetSetting(ConfigurationManager.AppSettings["CurrClient"], "CompCode", "00");
                if (module == null)
                {
                    module = "HRM";
                    UID = dc.deCode(Context.User.Identity.Name);
                }
                SqlConnection conn = new SqlConnection(dc.getConnStr(module));
                String sql = "";
                String rtn = "";
                sql = "select USRR_" + type + " from " + ConfigurationManager.AppSettings["PrivilegesDB"] + "defUserRight ";
                sql += " where Usrr_usr = @user";
                sql += " and usrr_form = @page";
                sql += " and usrr_cmpy = '" + CompCode + "' ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@user", SqlDbType.NVarChar, 30).Value = UID;
                cmd.Parameters.Add("@page", SqlDbType.NVarChar, 100).Value = Page.AppRelativeVirtualPath;
                rtn = dc.getValue(cmd);
                if (rtn == "Y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Child Method

        protected bool IsTextValidated(string strTextEntry)
        {
            Regex objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strTextEntry)
                 && (strTextEntry != "");
        }

        protected void ShowSearch(Boolean Display)
        {
            ImageButton lbSearch = (ImageButton)Parent.Parent.Parent.Parent.Parent.Parent.FindControl("lbSearch");
            if (lbSearch != null) lbSearch.Visible = Display ;
        }
        protected void ShowAdd(Boolean Display)
        {
            ShowAdd(Display, "N");
        }
        protected void ShowAdd(Boolean Display, String OverrideAccessCtrl)
        {
            ImageButton lbAdd = (ImageButton)Parent.Parent.Parent.Parent.Parent.Parent.FindControl("lbAdd");
            if (OverrideAccessCtrl == "Y")
                lbAdd.Visible = Display;
            else
                if (lbAdd != null) lbAdd.Visible = Display == false ? Display : GetAccessRight("ADD");
        }
        protected void ShowEdit(Boolean Display)
        {
            ShowEdit(Display, "N");
        }
        protected void ShowEdit(Boolean Display, String OverrideAccessCtrl)
        {
            ImageButton lbEdit = (ImageButton)Parent.Parent.Parent.Parent.Parent.Parent.FindControl("lbEdit");
            if (OverrideAccessCtrl == "Y")
                lbEdit.Visible = Display;
            else
                if (lbEdit != null) lbEdit.Visible = Display == false ? Display : GetAccessRight("CHG");
        }
        protected void ShowDelete(Boolean Display)
        {
            ShowDelete(Display, "N");
        }
        protected void ShowDelete(Boolean Display, String OverrideAccessCtrl)
        {
            ImageButton lbDelete = (ImageButton)Parent.Parent.Parent.Parent.Parent.Parent.FindControl("lbDelete");
            if (OverrideAccessCtrl == "Y")
                lbDelete.Visible = Display;
            else
                if (lbDelete != null) lbDelete.Visible = Display == false ? Display : GetAccessRight("DEL");
        }

        protected void ShowPrint(Boolean Display)
        {
            ShowPrint(Display, "");
        }

        protected void ShowPrint(Boolean Display, String PopupLink)
        {
            ImageButton lbPrint = (ImageButton)Parent.Parent.Parent.Parent.Parent.Parent.FindControl("lbPrint");
            if (lbPrint != null) 
            {
                lbPrint.Visible = Display;
                if (PopupLink.Length > 0)
                {
                    lbPrint.Attributes.Add("onclick", "javascript:window.open('" + PopupLink + "', 'PopupPrint','width=700, resizable=1, scrollbars=1, menubar=0, toolbar=1');");
                }
                }
        }

        #endregion

        #region Export Function for GridView
        public void ExGridViewToExcel(GridView ctl, String filename, String filetype)
        {
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename + "." + filetype);
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            ctl.AllowPaging = false;
            this.EnableViewState = false;
            ctl.DataBind();
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            ctl.RenderControl(hw);
            HttpContext.Current.Response.Write(tw.ToString());
            HttpContext.Current.Response.End();
        }

        public static void ExportExcel(string fileName, GridView[] gvs)   
        {   
            HttpContext.Current.Response.Clear();   
            HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));   
            HttpContext.Current.Response.ContentType = "application/ms-excel";   
                System.IO.StringWriter sw = new System.IO.StringWriter();   
            HtmlTextWriter htw = new HtmlTextWriter(sw);   
               
            foreach (GridView gv in gvs) {   
              //   Create a form to contain the grid   
              Table table = new Table();   
              table.GridLines = gv.GridLines;   
              //   add the header row to the table   
              if (!(gv.HeaderRow == null)) {   
                  PrepareControlForExport(gv.HeaderRow);   
                  table.Rows.Add(gv.HeaderRow);   
              }   
              //   add each of the data rows to the table   
              foreach (GridViewRow row in gv.Rows) {   
                  PrepareControlForExport(row);   
                  table.Rows.Add(row);   
              }   
              //   add the footer row to the table   
              if (!(gv.FooterRow == null)) {   
                  PrepareControlForExport(gv.FooterRow);   
                  table.Rows.Add(gv.FooterRow);   
              }   
              //   render the table into the htmlwriter   
              table.RenderControl(htw);   
            }   
            //   render the htmlwriter into the response   
            HttpContext.Current.Response.Write(sw.ToString());   
            HttpContext.Current.Response.End();   
      }   
      
        private static void PrepareControlForExport(Control control)   
        {   
            for (int i = 0; i < control.Controls.Count; i++)   
            {   
                Control current = control.Controls[i];   
                if (current is LinkButton)   
                {   
                    control.Controls.Remove(current);   
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));   
                }   
                else if (current is ImageButton)   
                {   
                    control.Controls.Remove(current);   
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));   
                }   
                else if (current is HyperLink)   
                {   
                    control.Controls.Remove(current);   
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));   
                }   
                else if (current is DropDownList)   
                {   
                    control.Controls.Remove(current);   
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));   
                }   
                else if (current is CheckBox)   
                {   
                    control.Controls.Remove(current);   
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));   
                }   
      
                if (current.HasControls())   
                {   
                    PrepareControlForExport(current);   
                }   
            }   
        }  
       #endregion
    }
}