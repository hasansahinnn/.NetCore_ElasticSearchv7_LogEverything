using ElasticSearchLibrary.Loggers.LogModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.Loggers.SearchModels
{
    public class EntitySearchModel : BaseFilter
    {
        public string EntityName { get; set; }
        public int UserId { get; set; }
        public int PrimaryKeyValue { get; set; }

        public ChangeState State { get; set; }

    }
}
