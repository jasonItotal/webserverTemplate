
using Newtonsoft.Json;
using System;

namespace Services.Model
{
    public class ApiManagementUser
    {
        public long ID { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        public string Token { get; set; }
        public DateTime? Expiry { get; set; }

        //for form editing use
        public string password { get; set; }
        public string confirm { get; set; }
    }
}