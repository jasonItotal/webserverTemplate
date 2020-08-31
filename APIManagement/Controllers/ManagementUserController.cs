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
using System.Web.Http;
using System.Web.Http.Cors;

namespace APIManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ManagementUserController : BaseController
    {
        private ManagementUserService managementUserService;
        public ManagementUserController() {

            managementUserService = new ManagementUserService(connectionString);
        }

        [HttpPost, HttpOptions]
        public JObject GetInfo(JObject parameter) {

            JObject response;
            response = GetValidatedResponse();
            //valid user token
            if (isCodeNormal(response)) {
                var token = parameter["Token"].ToString();
                var mUser = managementUserService.Get("", token);
                var data = ToJObject(mUser);
                var rolesArr = new JArray();
                rolesArr.Add("admin");
                data["roles"] = rolesArr;
                data["introduction"] = "test introduction";
                data["avatar"] = "https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif";
                response = CreateResponse(data);
            }
            return response;
        }
        [HttpGet, HttpOptions]
        public JObject List()
        {
            JObject response;
            response = GetValidatedResponse();
            if (isCodeNormal(response))
            {
                var parameters = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                var page = Convert.ToInt32(parameters["page"]);
                var limit = Convert.ToInt32(parameters["limit"]);
                var items = managementUserService.List(page,limit).ToArray();
                var total = managementUserService.Count();
                var data = ToJObject(items, total);
                response["data"] = data;
            }
            return response;
        }

        [HttpPost, HttpOptions]
        public JObject Logout(JObject parameter)
        {
            JObject response;
            response = GetValidatedResponse();
            //valid user token
            if (isCodeNormal(response))
            {
                var token = GetToken();
                var mUser = managementUserService.Get("", token);
                //reset token
                managementUserService.Update(mUser.ID.ToString(),"Null","Null");
            }
            return response;
        }

        [HttpPost, HttpOptions]
        public JObject Login(JObject parameter)
        {
            JObject response;
            string username = parameter["username"].ToString();
            string password = parameter["password"].ToString();
            ApiManagementUser mUser = managementUserService.Get("", "", username, password);
            if (mUser != null)
            {
                int aliveMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LoginAliveMinutes"]);
                mUser = managementUserService.GenerateToken(mUser.ID.ToString(), aliveMinutes);
                var data = ToJObject(mUser);
                response = CreateResponse(data);
            }
            else {
                response = CreateResponse(CODE_USER_PASSWORD_INCORRECT);
            }
            return response;
        }

        private string EncrpytPassword(string password) {
            //skip encrpyt password for debug.
            //implement encrpt later on
            return password;
        }

        [HttpPost, HttpOptions]
        public JObject Create(JObject parameter)
        {
            JObject response;
            response = GetValidatedResponse();
            //valid user token
            if (isCodeNormal(response))
            {
                var Name = parameter["Name"].ToString().Trim();
                var PasswordHash = EncrpytPassword(parameter["password"].ToString().Trim());
                var Token = "";
                var Expiry = "";
                var CR_USR = currentUser.Name;
                long newUserID = managementUserService.Insert(Name, PasswordHash,Token,Expiry,CR_USR);
                //if new user created, service would return a new user id.
                if (newUserID == 0) {
                    response = CreateResponse(CODE_CREATE_FAIL);
                }
            }
            return response;
        }

        [HttpPost, HttpOptions]
        public JObject Update(JObject parameter)
        {
            JObject response;
            response = GetValidatedResponse();
            //valid user token
            if (isCodeNormal(response))
            {
                var ID = parameter["ID"].ToString();
                var mUser = managementUserService.Get(ID);
                if (mUser != null)
                {
                    var Name = parameter["Name"].ToString();
                    var PasswordHash = EncrpytPassword(parameter["password"].ToString().Trim());
                    //reset token
                    managementUserService.Update(mUser.ID.ToString(), "", "", Name, PasswordHash, currentUser.Name);
                }
                else {
                    response = CreateResponse(CODE_UPDATE_FAIL);
                }
            }
            return response;
        }

        [HttpDelete, HttpOptions]
        public JObject Delete(JObject parameter)
        {
            JObject response;
            response = GetValidatedResponse();
            //valid user token
            if (isCodeNormal(response))
            {

                var ID = parameter["ID"].ToString();
                var mUser = managementUserService.Get(ID);
                if (mUser != null)
                {
                    managementUserService.Delete(mUser.ID.ToString(), currentUser.Name);
                }
                else
                {
                    response = CreateResponse(CODE_DELETE_FAIL);
                }
            }
            return response;
        }
    }
}
