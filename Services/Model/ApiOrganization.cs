
using Newtonsoft.Json;
using System;

namespace Services.Model
{
    public class ApiOrganization
    {
        public long ID { get; set; }
        public string IP { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Limitation { get; set; }
    }
}
