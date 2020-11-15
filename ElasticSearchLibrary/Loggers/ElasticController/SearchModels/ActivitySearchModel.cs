using ElasticSearchLibrary.Loggers.LogModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.Loggers.SearchModels
{
    public class ActivitySearchModel: BaseFilter
    {
        public int? UserId { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? HttpType { get; set; }
        public int? StatusCode { get; set; }
        public string ?IPAddress { get; set; }

    }
}
