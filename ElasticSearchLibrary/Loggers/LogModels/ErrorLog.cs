using ElasticSearchLibrary.Loggers.LogModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.Loggers.LogModels
{
    public class ErrorLog : IElasticModel
    {
        public int? UserId { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
