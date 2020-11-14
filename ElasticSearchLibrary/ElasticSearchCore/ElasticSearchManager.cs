using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchLibrary.ElasticSearchCore.Interfaces;
using ElasticSearchLibrary.ElasticSearchCore.LibraryConfiguration;
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


        #region Create Process

        public async Task<IBulkAliasResponse> CreateIndexAsync(string aliasName,string indexName) 
        {
            var exis = await elasticClient.IndexExistsAsync(indexName);
           
            if (exis.Exists)
                return new BulkAliasResponse();
            var newName = indexName + DateTime.Now.Ticks;
            var result = await elasticClient
                .CreateIndexAsync(newName,
                    ss =>
                        ss.Index(newName)
                            .Settings(
                                o => o.NumberOfShards(4).NumberOfReplicas(2).Setting("max_result_window", int.MaxValue)
                                         .Analysis(a => a
                        .TokenFilters(tkf => tkf.AsciiFolding("my_ascii_folding", af => af.PreserveOriginal(true)))
                        .Analyzers(aa => aa
                        .Custom("turkish_analyzer", ca => ca
                         .Filters("lowercase", "my_ascii_folding")
                         .Tokenizer("standard")))))
            );
            if (result.Acknowledged)
            {
                return await elasticClient.AliasAsync(al => al.Add(add => add.Index(newName).Alias(aliasName)));
            }
            throw new Exception($"Create Index {indexName} failed : :" + result.ServerError.Error.Reason);
        }

        public async Task<IBulkAliasResponse> CreateIndexAndAliasAsync(string indexName)
        {
            var exis = await elasticClient.IndexExistsAsync(indexName);

            if (exis.Exists)
                return new BulkAliasResponse();
            var newName = indexName + DateTime.Now.Ticks;
            var result = await elasticClient
                .CreateIndexAsync(newName,
                    ss =>
                        ss.Index(newName)
                            .Settings(
                                o => o.NumberOfShards(4).NumberOfReplicas(2).Setting("max_result_window", int.MaxValue)
                                         .Analysis(a => a
                        .TokenFilters(tkf => tkf.AsciiFolding("my_ascii_folding", af => af.PreserveOriginal(true)))
                        .Analyzers(aa => aa
                        .Custom("turkish_analyzer", ca => ca
                         .Filters("lowercase", "my_ascii_folding")
                         .Tokenizer("standard")))))
            );
            if (result.Acknowledged)
            {
                return await elasticClient.AliasAsync(al => al.Add(add => add.Index(newName).Alias(indexName)));
            }
            throw new Exception($"Create Index {indexName} failed : :" + result.ServerError.Error.Reason);
        }
  
        public Task<IBulkAliasResponse> CreateAliasAsync(string aliasName,string? IndexName)
        {
           return elasticClient.AliasAsync(x => x.Add(new AliasAddAction() { Add = new AliasAddOperation() { Alias = aliasName, Index = IndexName } }));
        }
        #endregion


        #region Insert Process

        /// <summary>
        ///     Log Insert Function with String IndexType
        /// </summary>
        public async Task<IIndexResponse> CheckExistsAndInsert<T>(IndexType IndexType, T logs) where T : class
        { 
            string indexName = IndexType.ToString();
            return await CheckExistsAndInsert<T>(indexName,logs);
        }

        /// <summary>
        ///     Log Insert Function with String Index Name
        /// </summary>
        public async Task<IIndexResponse> CheckExistsAndInsert<T>(string indexName, T logs) where T : class
        {
            if (!elasticClient.IndexExists(indexName).Exists)
            {
               await CreateIndexAndAliasAsync(indexName);
            }
            return await elasticClient.IndexAsync<T>(logs, idx => idx.Index(indexName));
        }

        #endregion


        #region Clear Data Process

        /// <summary>
        ///     Clear Logs  with IndexType
        /// </summary>
        public Task<IDeleteByQueryResponse> ClearIndexData(IndexType IndexType)
        {
            string indexName = IndexType.ToString();
            return ClearIndexData(indexName);
        }

        /// <summary>
        ///     Clear Index Logs  with string
        /// </summary>
        public Task<IDeleteByQueryResponse> ClearIndexData(string indexName) 
        {
            DeleteByQueryRequest r = new DeleteByQueryRequest(new IndexName() { Name = indexName });
            r.QueryOnQueryString = "*";
            return elasticClient.DeleteByQueryAsync(r);
        }

        #endregion


        #region Delete Process

        /// <summary>
        /// Delete Index
        /// </summary>
        public  Task<IDeleteIndexResponse> DeleteIndex(string indexName) 
        {
            return elasticClient.DeleteIndexAsync(indexName);
        }

        /// <summary>
        ///   Delete Alias. If you want to delete Indexs too set Delete paramater true. 
        /// </summary>
        public Task<IBulkAliasResponse> DeleteAlias(string aliasName,bool DeleteAllIndexs)
        {
            var exist = elasticClient.AliasExists(new AliasExistsRequest(aliasName.ToIndices())).Exists;
            if (!exist) return null;
            if (DeleteAllIndexs)
                foreach (var item in GetAllIndexByAlias(aliasName).Result)
                    DeleteIndex(item);
            return elasticClient.AliasAsync(a => a.Remove(t => t.Alias(aliasName).Index("*")));
        }


        #endregion



       #region Search Process

        public IReadOnlyCollection<ActivityLog> SearchActivityLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "",string httpMethod="", int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.activity_log,string indexNameStr = "")
        {
            throw new NotImplementedException();
        }

        public  IReadOnlyCollection<ErrorLog> SearchChangeLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string className = "", ChangeState operation = ChangeState.Updated, int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.entity_log, string indexNameStr = "")
        {
            throw new NotImplementedException();
        }

        public  IReadOnlyCollection<ErrorLog> SearchErrorLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "", string method = "", int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.error_log, string indexNameStr = "")
        {
            throw new NotImplementedException();
        }

        #endregion



        #region Get Process

        /// <summary>
        ///     Get Alias Definition using Index Name
        /// </summary>
        public async Task<IEnumerable<AliasDefinition>> GetAliasByIndex(string indexName)
        {
            var result = elasticClient.GetAliasesPointingToIndex(indexName);
            return result;
        }

        /// <summary>
        ///     Get All Index Names using AliasName
        /// </summary>
        public async Task<IEnumerable<string>> GetAllIndexByAlias(string aliasName)
        {
            var result = elasticClient.GetIndicesPointingToAlias(aliasName);
            return result;
        }


        /// <summary>
        /// Move Documents to New Index 
        /// </summary>
        public Task<IReindexOnServerResponse> ReIndex(string sourceIndexName, string destinationIndexName)
        {
            var reindexResponse = elasticClient.ReindexOnServerAsync(r => r
                         .Source(s => s
                             .Index(sourceIndexName)
                         )
                         .Destination(d => d
                             .Index(destinationIndexName)
                         )
                         .WaitForCompletion()
                     );
            return reindexResponse;
        }


        #endregion

    }


    /*
        
     Alias Exist Check: elasticClient.GetAlias(a => a.Name(indexName)).Indices.Count   
        
    */



}
