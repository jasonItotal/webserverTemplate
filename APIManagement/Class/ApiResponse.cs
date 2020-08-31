using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIManagement.Class
{
    public class ApiResponse
    {
        private bool debug = true;

        public int code { get; set; }
        public JObject data { get; set; }
        public void setData(string name, string message)
        {
            if (this.data == null)
            {
                this.data = new JObject();
            }
            this.data[name] = message;
        }
        public const int INSERT_SUCCESS = 100;
        public const int UPDATE_SUCCESS = 200;
        public const int DELETE_SUCCESS = 300;
        public const int INSERT_FAILED = 500;
        public const int UPDATE_FAILED = 600;
        public const int DELETE_FAILED = 700;

        public const int NORMAL = 20000;
        public const int FAILED = 50000;
        public const int FAILED_ITEM_NOT_FOUND = 50001;
        public const int FAILED_ITEM_EXIST = 50002;
        public const int FAILED_TOKEN_INVALID = 50003;
        public const int FAILED_EXCEPTION = 51000;
        public const int LOGIN_FAILED = 900;
    }
}