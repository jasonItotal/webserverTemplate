
using Newtonsoft.Json;
using System;
namespace Services.Model
{
    public class ApiBlackList : BaseModel
    {
        public long ID { get; set; }
        public long UserID { get; set; }

        //for display
        public string User_Name { get; set; }
    }
}
