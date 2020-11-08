using System;
using System.Linq;
using ElasticSearch.Configuration;
using ElasticSearch.ElasticSearchManager;
using ElasticSearch.ElasticSearchManager.LogAttributes.EntityAttributes;
using Microsoft.AspNetCore. Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace ElasticSearch.Models
{
    public class ProductContext : DbContext
    {
        IHttpContextAccessor httpcontext; // For User ID
        public DbSet<Product> Products { get; set; }
        private readonly IElasticSearchManager<ChangeLog> elasticSearchService;
        public ProductContext(IElasticSearchManager<ChangeLog> _elasticSearchService, DbContextOptions options, IHttpContextAccessor _httpcontext) : base(options) 
        {
            this.elasticSearchService = _elasticSearchService;
            this.httpcontext =_httpcontext; 
        }
 
        public override int SaveChanges()
        {
            try
            {
                var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList(); // Updated Records
                var deletedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted).ToList(); // Deleted Records
                var userId = httpcontext.HttpContext.GetAuthUser(); // Get Auth User
                var now = System.DateTime.UtcNow;
                foreach (var change in modifiedEntities) // Modified Data
                {
                    bool LogModelState = change.Entity.GetType().CheckAttributeExist<LogModelAttribute>(); // Check LogModel Attribute

                    var entityName = change.Entity.GetType().Name; // Table name
                    var PrimaryKey = change.OriginalValues.Properties.FirstOrDefault(prop => prop.IsPrimaryKey() == true).Name; //Table Primary Key

                    ChangeLog log = new ChangeLog(userId, entityName, change.OriginalValues[PrimaryKey].ToInt(), now, ChangeState.Updated);
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
                    elasticSearchService.CheckExistsAndInsert(IndexType.change_log, log); // Send to Elastic
                }

                foreach (var deleted in deletedEntities) // Deleted Data
                {
                    var entityName = deleted.Entity.GetType().Name; // Table name
                    var PrimaryKey = deleted.OriginalValues.Properties.FirstOrDefault(prop => prop.IsPrimaryKey() == true).Name; //Table Primary Key
                    ChangeLog log = new ChangeLog(userId, entityName, deleted.OriginalValues[PrimaryKey].ToInt(), now, ChangeState.Deleted);
                    elasticSearchService.CheckExistsAndInsert(IndexType.change_log, log); // Send to Elastic
                }
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

     
    }
   
}
