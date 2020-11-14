using ElasticSearchLibrary.Loggers.LogModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.Loggers.LogModels
{
    public class ChangeLog : IElasticModel
    {
        public ChangeLog(int? userId,string entityName, int primaryKeyValue,DateTime date, ChangeState state)
        {
            this.UserId = userId;
            this.EntityName = entityName;
            this.PrimaryKeyValue = primaryKeyValue;
            this.DateChanged = date;
            this.State = state;
            ChangedValues = new List<ChangedValues>();
        }
        public string EntityName { get; set; }
        public int? UserId { get; set; }
        public int PrimaryKeyValue { get; set; }
        public List<ChangedValues> ChangedValues { get; set; }
        public System.DateTime DateChanged { get; set; }

        public ChangeState State { get; set; }
    }
    public partial class ChangedValues
    {
        public ChangedValues(string propertyName, string oldValue, string newValue)
        {
            this.PropertyName = propertyName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
    public enum ChangeState
    {
        Updated = 1,
        Deleted = 2,
        Added = 3
    }
   
}
  