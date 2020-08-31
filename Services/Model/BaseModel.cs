using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Model
{
    public class BaseModel
    {

        
        public string CR_USR { get; set; }
        
        public DateTime CR_DATE { get; set; }
        public string UP_USR { get; set; }
        
        public DateTime? UP_DATE { get; set; }
        
        public string DEL_USR { get; set; }
        [JsonIgnore]
        public DateTime? DEL_DATE { get; set; }
        [JsonIgnore]
        public string DELETED { get; set; }
    }
}
