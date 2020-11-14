using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchLibrary.ElasticSearchCore.Interfaces;
using ElasticSearchLibrary.Loggers.LogModels;
using Microsoft.Extensions.Configuration;
using Nest;

namespace ElasticSearchLibrary.ElasticSearchCore
{
    
    public class ElasticSearchManager:IElasticSearchManager
    {
        IElasticClientProvider elasticProvider;
        ElasticClient elasticClient;

        public ElasticSearchManager(IElasticClientProvider provider)
        {
            elasticProvider = provider;
            elasticClient = provider.ElasticClient;
        }



        #region Insert Process

        /// <summary>
        ///     Log Insert Function with String IndexType
        /// </summary>
        public Task CheckExistsAndInsert<T>(IndexType IndexType, T logs) where T : class
        {
            string indexName = IndexType.ToString();
            if (!elasticClient.IndexExists(indexName).Exists)
            {
                var newIndexName = indexName + System.DateTime.Now.Ticks;
                var indexSettings = new IndexSettings();
                indexSettings.NumberOfReplicas = 2; 
                indexSettings.NumberOfShards = 3;
                /*
                 * By default, Elasticsearch creates 1 primary shard and 1 replica for each index. 
                 * Allocating multiple shards and replicas is the essence of the design for distributed search capability, providing for high availability and quick access in searches against the documents within an index. 
                 * The main difference between a primary and a replica shard is that only the primary shard can accept indexing requests. 
                 * Both replica and primary shards can serve querying requests.
                */
                var createIndexDescriptor = new CreateIndexDescriptor(newIndexName)
                    .Mappings(ms => ms.Map<T>(m => m.AutoMap()))
                    .InitializeUsing(new IndexState() { Settings = indexSettings })
                    .Aliases(a => a.Alias(indexName));

                var response = elasticClient.CreateIndex(createIndexDescriptor);
            }
            return elasticClient.IndexAsync(logs, idx => idx.Index(indexName));
        }

        /// <summary>
        ///     Log Insert Function with String Index Name
        /// </summary>
        public Task CheckExistsAndInsert<T>(string indexName, T logs) where T : class
        {
            if (!elasticClient.IndexExists(indexName).Exists)
            {
                var newIndexName = indexName + System.DateTime.Now.Ticks;
                var indexSettings = new IndexSettings();
                indexSettings.NumberOfReplicas = 2;
                indexSettings.NumberOfShards = 3;
                var createIndexDescriptor = new CreateIndexDescriptor(newIndexName)
                    .Mappings(ms => ms.Map<T>(m => m.AutoMap()))
                    .InitializeUsing(new IndexState() { Settings = indexSettings })
                    .Aliases(a => a.Alias(indexName));

                var response = elasticClient.CreateIndex(createIndexDescriptor);
            }
            return elasticClient.IndexAsync(logs, idx => idx.Index(indexName));
        }

        #endregion


        #region Clear Process

        /// <summary>
        ///     Clear Logs  with IndexType
        /// </summary>
        public async Task ClearIndexData<T>(IndexType IndexType, T logs) where T : class, new()
        {
            string indexName = IndexType.ToString();
            await DeleteIndex(indexName);
            await CheckExistsAndInsert<T>(indexName, new T());
        }

        /// <summary>
        ///     Clear Logs  with string
        /// </summary>
        public async Task ClearIndexData<T>(string indexName,T logs) where T : class,new()
        {
            await DeleteIndex(indexName);
            await CheckExistsAndInsert<T>(indexName,new T());
        }

        #endregion


        #region Delete Process
        /// <summary>
        /// Delete Index
        /// </summary>
        public Task DeleteIndex(string indexName) 
        {
            return elasticClient.DeleteIndexAsync(indexName);
        }
        /// <summary>
        /// Delete Index And Create Again
        /// </summary>
        public async Task ReIndex<T>(string indexName, T logs) where T : class, new()
        {
            await DeleteIndex(indexName);
            await CheckExistsAndInsert<T>(indexName, new T());
        }

        #endregion



       #region Search Process

        public IReadOnlyCollection<ActivityLog> SearchActivityLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "",string httpMethod="", int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.activity_log,string indexNameStr = "")
        {
            throw new NotImplementedException();
        }

        public  IReadOnlyCollection<ErrorLog> SearchChangeLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string className = "", ChangeState operation = ChangeState.Updated, int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.change_log, string indexNameStr = "")
        {
            throw new NotImplementedException();
        }

        public  IReadOnlyCollection<ErrorLog> SearchErrorLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "", string method = "", int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.error_log, string indexNameStr = "")
        {
            throw new NotImplementedException();
        }

        #endregion



    }
    public static class ElasticManagerExtension
    {
        public static IElasticSearchManager GetElasticManager(this IConfiguration configuration)
        {
            return configuration.Get<IElasticSearchManager>();
        }
    }
}
