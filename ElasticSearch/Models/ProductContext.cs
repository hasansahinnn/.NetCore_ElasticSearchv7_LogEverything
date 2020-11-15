using System;
using System.Linq;
using ElasticSearch.Configuration;
using ElasticSearchLibrary;
using ElasticSearchLibrary.Loggers.DBEntityLogger;
using Microsoft.AspNetCore. Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace ElasticSearch.Models
{
    public class ProductContext : DbContext
    {
        private IDatabaseLogger dbLogger;
        public DbSet<Product> Products { get; set; }
        public ProductContext(DbContextOptions options, IDatabaseLogger _dbLogger) : base(options) 
        {
            this.dbLogger = _dbLogger; 
        }
 
        public override int SaveChanges()
        {
            try
            {
                dbLogger.LogChanges(ChangeTracker);  // For DB Entity Logging
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

     
    }
   
}
