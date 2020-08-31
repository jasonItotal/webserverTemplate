
using Newtonsoft.Json;
using System;
namespace Services.Model
{
    public class ApiLog : BaseModel
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public string Action { get; set; }
        public DateTime? ActionStart { get; set; }
        public DateTime? ActionEnd { get; set; }

        //for list display
        public string User_Name { get; set; }
    }
}
