using ElasticSearchLibrary.Loggers.LogAttributes.EntityAttributes;
using ElasticSearchLibrary.Loggers.LogModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Models
{
    [LogModel]
    public class MyTestModel : IElasticModel //  <-- Optional Interface for Logging Models
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public MyTestModel(int ıd, string text)
        {
            Id = ıd;
            Text = text;
        }
    }
}
