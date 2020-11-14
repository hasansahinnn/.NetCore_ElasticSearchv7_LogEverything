using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.Loggers.SearchModels
{
    public class BaseFilter
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 0;
        public int RowCount { get; set; } = 10;
    }
}
