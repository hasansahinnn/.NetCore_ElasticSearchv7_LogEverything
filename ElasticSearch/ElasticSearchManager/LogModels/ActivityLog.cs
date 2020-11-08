using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.ElasticSearchManager
{
    public class ActivityLog
    {
        public int UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Params { get; set; }
        public string IPAddress { get; set; }
        public System.DateTime DateCreated { get; set; }
    }
}
