using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;

namespace APIManagement.Controllers
{
    public class BaseController : ApiController
    {
        protected int CODE_NORMAL = 20000;
        protected int CODE_TOKEN_EXPIRED = 50014;
        protected string CODE_TOKEN_EXPIRED_DESC = "Token expired.";
        protected int CODE_ILLEGAL_TOKEN = 50008;
        protected string CODE_ILLEGAL_TOKEN_DESC = "Illegal token.";
        protected int CODE_OTHER_CLIENT_LOGGED_IN = 50012;
        protected string CODE_OTHER_CLIENT_LOGGED_IN_DESC = "Other client logged in.";
        protected int CODE_USER_PASSWORD_INCORRECT = 60204;
        protected string CODE_USER_PASSWORD_INCORRECT_DESC = "Account and password are incorrect.";

        protected int CODE_CREATE_FAIL = 70001;
        protected string CODE_CREATE_FAIL_DESC = "Create Failed.";
        protected int CODE_UPDATE_FAIL = 70002;
        protected string CODE_UPDATE_FAIL_DESC = "Update Failed.";
        protected int CODE_DELETE_FAIL = 70003;
        protected string CODE_DELETE_FAIL_DESC = "Delete Failed.";

        protected ManagementUserService mUserService;
        protected string connectionString;
        protected ApiManagementUser currentUser;


        public BaseController() {
            connectionString = ConfigurationManager.AppSettings["connectionString"];
            mUserService = new ManagementUserService(connectionString);
        }
        public JObject ToJObject(Array items, long total)
        {
            var obj = new JObject();
            obj["items"] = JArray.Parse(JsonConvert.SerializeObject(items));
            obj["total"] = total;
            return obj;
        }

        public JObject ToJObject(Object obj) {
            if (obj == null) {
                return new JObject();
            }
            return JObject.Parse(JsonConvert.SerializeObject(obj));
        }


        public int ValidateToken() {
            int code = CODE_ILLEGAL_TOKEN;
            string token = GetToken();
            //validate token
            if (!string.IsNullOrEmpty(token)) {
                //get management user by token
                ApiManagementUser mUser = mUserService.Get("",token);
                if (mUser == null)
                {
                    code = CODE_ILLEGAL_TOKEN;
                }
                else {
                    if (mUser.Expiry != null)
                    {
                        int res = DateTime.Compare(mUser.Expiry.Value, DateTime.Now);
                        //it means token expiried.
                        if (res < 0)
                        {
                            code = CODE_TOKEN_EXPIRED;
                        }
                        else {
                            currentUser = mUser;
                            code = CODE_NORMAL;
                        }
                    }
                }
            }
            return code;
        }

        public bool isCodeNormal(JObject response)
        {
            var code = response["code"].ToString();
            if (Convert.ToInt32(code) == CODE_NORMAL)
            {
                return true;
            }
            return false;
        }
        public bool isCodeNormal(int code) {
            if (code != CODE_NORMAL) {
                return true;
            }
            return false;
        }

        public JObject GetValidatedResponse() {
            int code = ValidateToken();
            var response = CreateResponse(code);
            return response;
        }

        public JObject CreateResponse(JObject data)
        {
            var response = CreateResponse(CODE_NORMAL);
            response["data"] = data;
            return response;
        }
        public JObject CreateResponse(int code) {
            JObject response = new JObject();
            response["code"] = code;
            if (code == CODE_ILLEGAL_TOKEN)
            {
                response["message"] = CODE_ILLEGAL_TOKEN_DESC;
            }
            if (code == CODE_TOKEN_EXPIRED)
            {
                response["message"] = CODE_TOKEN_EXPIRED_DESC;
            }
            if (code == CODE_USER_PASSWORD_INCORRECT)
            {
                response["message"] = CODE_USER_PASSWORD_INCORRECT_DESC;
            }
            if (code == CODE_CREATE_FAIL)
            {
                response["message"] = CODE_CREATE_FAIL_DESC;
            }
            if (code == CODE_UPDATE_FAIL)
            {
                response["message"] = CODE_UPDATE_FAIL_DESC;
            }
            if (code == CODE_DELETE_FAIL)
            {
                response["message"] = CODE_DELETE_FAIL_DESC;
            }
            return response;
        }

        public string GetToken()
        {
            string admin_token = "";
            var headers = Request.Headers;
            if (headers.Contains("X-Token"))
            {
                admin_token = headers.GetValues("X-Token").First();
            }
            return admin_token;
        }
    }
}
