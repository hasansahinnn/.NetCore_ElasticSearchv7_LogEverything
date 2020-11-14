using ElasticSearchLibrary.Loggers.LogAttributes.EntityAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Models
{
    //[LogModel]  // Log All Props
    public class Product
    {
        [Key]
        [DisplayName("Record Id")] // For clarity when displaying ChangeLog records
        public int ID { get; set; }
        [LogProp]  // Log Specific Props
        public string Name { get; set; }
        [DisplayName("Seri No")]
        [LogProp]
        public string SeriNo { get; set; }
        [DisplayName("Total Count")]
        [LogProp]
        public int TotalCount { get; set; }
        [LogProp]
        public decimal Price { get; set; }
        public string WarehouseAddress { get; set; }
    }
}
