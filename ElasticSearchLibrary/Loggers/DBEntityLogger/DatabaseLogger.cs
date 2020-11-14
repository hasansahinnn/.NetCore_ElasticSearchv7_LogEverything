using ElasticSearchLibrary.ElasticSearchCore;
using ElasticSearchLibrary.ElasticSearchCore.LibraryConfiguration;
using ElasticSearchLibrary.Loggers.LogAttributes.EntityAttributes;
using ElasticSearchLibrary.Loggers.LogModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElasticSearchLibrary.Loggers.DBEntityLogger
{
    public  class DatabaseLogger: IDatabaseLogger
    {
        private  IElasticSearchManager elasticSearchService;
        private IHttpContextAccessor httpcontext;
        public DatabaseLogger(IElasticSearchManager _elasticSearchService, IHttpContextAccessor _httpcontext)
        {
            this.elasticSearchService = _elasticSearchService;
            this.httpcontext = _httpcontext;
        }
        public void LogChanges(ChangeTracker changes)
        {
            var modifiedEntities = changes.Entries().Where(p => p.State == EntityState.Modified).ToList(); // Updated Records
            var deletedEntities = changes.Entries().Where(p => p.State == EntityState.Deleted).ToList(); // Deleted Records
            var userId = httpcontext.HttpContext.GetAuthUser(); // Get Auth User
            var now = System.DateTime.UtcNow;
            foreach (var change in modifiedEntities) // Modified Data
            {
                bool LogModelState = change.Entity.GetType().CheckAttributeExist<LogModelAttribute>(); // Check LogModel Attribute

                var entityName = change.Entity.GetType().Name; // Table name
                var PrimaryKey = change.OriginalValues.Properties.FirstOrDefault(prop => prop.IsPrimaryKey() == true).Name; //Table Primary Key

                EntityLog log = new EntityLog(userId, entityName, change.OriginalValues[PrimaryKey].ToInt(), now, ChangeState.Updated);
                foreach (IProperty prop in change.OriginalValues.Properties) // Updated Props
                {
                    bool LogPropState = prop.PropertyInfo.CheckAttributeExist<LogPropAttribute>(); // Check LogProp Attribute
                    if (!LogModelState && !LogPropState) continue;

                    var originalValue = change.OriginalValues[prop.Name].ToString();
                    var currentValue = change.CurrentValues[prop.Name]?.ToString();
                    string a = prop.GetDisplayName();
                    if (originalValue != currentValue)
                    {
                        log.ChangedValues.Add(new ChangedValues(prop.GetDisplayName(), originalValue, currentValue));
                    }
                }
                elasticSearchService.CheckExistsAndInsert<EntityLog>(IndexType.entity_log, log); // Send to Elastic
            }

            foreach (var deleted in deletedEntities) // Deleted Data
            {
                var entityName = deleted.Entity.GetType().Name; // Table name
                bool LogModelState = deleted.Entity.GetType().CheckAttributeExist<LogModelAttribute>(); // Check LogModel Attribute
                if (!LogModelState) continue;
                var PrimaryKey = deleted.OriginalValues.Properties.FirstOrDefault(prop => prop.IsPrimaryKey() == true).Name; //Table Primary Key
                EntityLog log = new EntityLog(userId, entityName, deleted.OriginalValues[PrimaryKey].ToInt(), now, ChangeState.Deleted);
                elasticSearchService.CheckExistsAndInsert<EntityLog>(IndexType.entity_log, log); // Send to Elastic
            }
        }
    }
}
