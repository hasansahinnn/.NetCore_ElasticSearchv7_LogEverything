using ElasticSearchLibrary.Loggers.LogModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.Loggers.SearchModels
{
    public class ErrorSearchModel : BaseFilter
    {
        public int UserId { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

    }
}
