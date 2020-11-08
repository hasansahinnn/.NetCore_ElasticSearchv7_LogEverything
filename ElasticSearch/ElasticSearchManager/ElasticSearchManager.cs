using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Models;
using Nest;

namespace ElasticSearch.ElasticSearchManager
{
    
    public  class ElasticSearchManager<T>:IElasticSearchManager<T> where T : class
    {
        ElasticClientProvider elasticProvider;
        ElasticClient elasticClient;

        public ElasticSearchManager(ElasticClientProvider provider)
        {
            elasticProvider = provider;
            elasticClient = provider.ElasticClient;
        }

        /// <summary>
        ///     Save Log
        /// </summary>
        public  void CheckExistsAndInsert(IndexType IndexType, T logs) 
        {
            string indexName = IndexType.ToString();
            if (!elasticClient.IndexExists(indexName).Exists)
            {
                var newIndexName = indexName + System.DateTime.Now.Ticks;
                var indexSettings = new IndexSettings();
                indexSettings.NumberOfReplicas = 1; 
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
            elasticClient.Index(logs, idx => idx.Index(indexName));
        }

        public  IReadOnlyCollection<ActivityLog> SearchActivityLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "", int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.activity_log)
        {
            throw new NotImplementedException();
        }

        public  IReadOnlyCollection<ErrorLog> SearchChangeLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string className = "", ChangeState operation = ChangeState.Updated, int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.change_log)
        {
            throw new NotImplementedException();
        }

        public  IReadOnlyCollection<ErrorLog> SearchErrorLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "", string method = "", int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.error_log)
        {
            throw new NotImplementedException();
        }

       
    }
  
}
