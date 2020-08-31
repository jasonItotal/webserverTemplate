using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using iTotal.CorePart;

namespace iTotal.CorePart
{
    public class CoreParts : System.Web.UI.UserControl, IWebPart
    {

        #region Declare Varibles
        // add the IWebPart interface to the class
        private string _catalogImageUrl = string.Empty;
        private string _description = string.Empty;
        private string _subTitle = string.Empty;
        private string _title = string.Empty;
        private string _icon = "~/Images/i-TotalService-3.ico";

        //public Label LastIP;
        public string CatalogIconImageUrl
        {
            get { return _catalogImageUrl; }
            set { _catalogImageUrl = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [Personalizable()]
        [WebBrowsable()]
        public string Subtitle
        {
            get { return _subTitle; }
            set { _subTitle = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string TitleIconImageUrl
        {
            get { return _icon; }
            set { _icon = value; }
        }

        public string TitleUrl
        {
            get { return string.Empty; }
            set { ; }
        }
        #endregion

        //public static void AddDelMsg(ImageButton lb, String value)
        //{
        //    if (lb != null) { lb.Attributes.Add("onclick", "return confirm(\"" + Resources.ErrMsg.M1003 + " - " + value + " ?\")"); }
        //}

        public string GetRemoteIP()
        {
            return Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}