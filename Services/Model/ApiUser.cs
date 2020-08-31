
using Newtonsoft.Json;
using System;

namespace Services.Model
{
    public class ApiUser
    {
        public long ID { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        public string ClientName { get; set; }
        [JsonIgnore]
        public string ClientPasswordHash { get; set; }
        //public string IP { get; set; }
        public string Domain { get; set; }
        public long? OrganizationID { get; set; }
        public string Token { get; set; }
        public DateTime? Expiry { get; set; }

        //for display
        public string Organization_Name { get; set; }

        //for form generator
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string ClientPassword { get; set; }
        public string ClientPasswordConfirm { get; set; }
    }
}