using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.ElasticSearchManager
{
    public class ErrorLog
    {
        public int? UserId { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
