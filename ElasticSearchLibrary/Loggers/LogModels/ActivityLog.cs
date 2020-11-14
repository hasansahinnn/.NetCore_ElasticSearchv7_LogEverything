using ElasticSearchLibrary.Loggers.LogModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.Loggers.LogModels
{
    public class ActivityLog : IElasticModel
    {
        public int UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string HttpType { get; set; } 
        public int StatusCode { get; set; }
        public string Params { get; set; }
        public string IPAddress { get; set; }
        public System.DateTime DateCreated { get; set; }
    }
}
