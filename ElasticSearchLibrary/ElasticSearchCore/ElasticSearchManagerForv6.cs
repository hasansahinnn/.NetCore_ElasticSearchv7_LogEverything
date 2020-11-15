/*   Elastic Search v5 - v6 Manager Class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchLibrary.ElasticSearchCore.Interfaces;
using ElasticSearchLibrary.ElasticSearchCore.LibraryConfiguration;
using ElasticSearchLibrary.Loggers.LogModels;
using ElasticSearchLibrary.Loggers.SearchModels;
using Microsoft.Extensions.Configuration;
using Nest;

namespace ElasticSearchLibrary.ElasticSearchCore
{

    public class ElasticSearchManager : IElasticSearchManager
    {
        IElasticClientProvider elasticProvider;
        ElasticClient elasticClient;

        public ElasticSearchManager(IElasticClientProvider provider)
        {
            elasticProvider = provider;
            elasticClient = provider.ElasticClient;
        }

        public async Task<IBulkAliasResponse> CreateIndexAsync(string aliasName, string indexName)
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

        public Task<IBulkAliasResponse> CreateAliasAsync(string aliasName, string? IndexName)
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
            return await CheckExistsAndInsert<T>(indexName, logs);
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
        public Task<IDeleteIndexResponse> DeleteIndex(string indexName)
        {
            return elasticClient.DeleteIndexAsync(indexName);
        }

        /// <summary>
        ///   Delete Alias. If you want to delete Indexs too set Delete paramater true. 
        /// </summary>
        public Task<IBulkAliasResponse> DeleteAlias(string aliasName, bool DeleteAllIndexs)
        {
            var exist = elasticClient.AliasExists(new AliasExistsRequest(aliasName.ToIndices())).Exists;
            if (!exist) return null;
            if (DeleteAllIndexs)
                foreach (var item in GetAllIndexByAlias(aliasName).Result)
                    DeleteIndex(item);
            return elasticClient.AliasAsync(a => a.Remove(t => t.Alias(aliasName).Index("*")));
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


        #region Search Process

        public async Task<IReadOnlyCollection<ActivityLog>> SearchActivityLogs(ActivitySearchModel activityFilter)
        {
            activityFilter.BeginDate = activityFilter.BeginDate == null ? DateTime.Parse("01/01/1900") : activityFilter.BeginDate;
            activityFilter.EndDate = activityFilter.EndDate == null ? DateTime.Now : activityFilter.EndDate;
            var response = elasticClient.Search<ActivityLog>(s => s
            .From(activityFilter.Page)
            .Size(activityFilter.RowCount)
            .Sort(ss => ss.Descending(p => p.DateCreated))
            .Query(q => q
                .Bool(b => b
                    .Must(
                        q => q.Term(t => t.UserId, activityFilter.UserId),
                        q => q.Term(t => t.ControllerName.ToFilter(), activityFilter.ControllerName.ToFilter()),
                        q => q.Term(t => t.ActionName.ToFilter(), activityFilter.ActionName.ToFilter()),
                        q => q.Term(t => t.HttpType.ToFilter(), activityFilter.HttpType.ToFilter()),
                        q => q.Term(t => t.IPAddress.ToFilter(), activityFilter.IPAddress.ToFilter()),
                        q => q.Term(t => t.StatusCode, activityFilter.StatusCode),
                         q => q.DateRange(r => r
                        .Field(f => f.DateCreated)
                        .GreaterThanOrEquals(DateMath.Anchored(((DateTime)activityFilter.BeginDate).AddDays(-1)))
                        .LessThanOrEquals(DateMath.Anchored(((DateTime)activityFilter.EndDate).AddDays(1)))
                        ))
                     )
                  )
            .Index(IndexType.activity_log.ToString())
            );
            return response.Documents;
        }

        public async Task<IReadOnlyCollection<ErrorLog>> SearchErrorLogs(ErrorSearchModel errorFilter)
        {
            errorFilter.BeginDate = errorFilter.BeginDate == null ? DateTime.Parse("01/01/1900") : errorFilter.BeginDate;
            errorFilter.EndDate = errorFilter.EndDate == null ? DateTime.Now : errorFilter.EndDate;
            var response = elasticClient.Search<ErrorLog>(s => s
            .From(errorFilter.Page)
            .Size(errorFilter.RowCount)
            .Sort(ss => ss.Descending(p => p.DateCreated))
            .Query(q => q
                .Bool(b => b
                    .Must(
                        q => q.Term(t => t.UserId, errorFilter.UserId),
                        q => q.Term(t => t.Path.ToFilter(), errorFilter.Path.ToFilter()),
                        q => q.Term(t => t.Method.ToFilter(), errorFilter.Method.ToFilter()),
                        q => q.Term(t => t.StatusCode, errorFilter.StatusCode),
                         q => q.DateRange(r => r
                        .Field(f => f.DateCreated)
                        .GreaterThanOrEquals(DateMath.Anchored(((DateTime)errorFilter.BeginDate).AddDays(-1)))
                        .LessThanOrEquals(DateMath.Anchored(((DateTime)errorFilter.EndDate).AddDays(1)))
                        ))
                     )
                  )
            .Index(IndexType.error_log.ToString())
            );
            return response.Documents;
        }

        public async Task<IReadOnlyCollection<EntityLog>> SearchEntityLogs(EntitySearchModel entityFilter)
        {
            entityFilter.BeginDate = entityFilter.BeginDate == null ? DateTime.Parse("01/01/1900") : entityFilter.BeginDate;
            entityFilter.EndDate = entityFilter.EndDate == null ? DateTime.Now : entityFilter.EndDate;
            var response = elasticClient.Search<EntityLog>(s => s
            .From(entityFilter.Page)
            .Size(entityFilter.RowCount)
            .Sort(ss => ss.Descending(p => p.DateChanged))
            .Query(q => q
                .Bool(b => b
                    .Must(
                        q => q.Term(t => t.UserId, entityFilter.UserId),
                        q => q.Term(t => t.EntityName.ToFilter(), entityFilter.EntityName.ToFilter()),
                        q => q.Term(t => t.PrimaryKeyValue, entityFilter.PrimaryKeyValue),
                        q => q.Term(t => t.State, entityFilter.State),
                         q => q.DateRange(r => r
                        .Field(f => f.DateChanged)
                        .GreaterThanOrEquals(DateMath.Anchored(((DateTime)entityFilter.BeginDate).AddDays(-1)))
                        .LessThanOrEquals(DateMath.Anchored(((DateTime)entityFilter.EndDate).AddDays(1)))
                        ))
                     )
                  )
            .Index(IndexType.entity_log.ToString())
            );
            return response.Documents;
        }

        public async Task<IReadOnlyCollection<T>> SearchIndexLogs<T>(GlobalSearchModel globalFilter) where T : class
        {
            var response = elasticClient.Search<T>(s => s
            .From(globalFilter.Page)
            .Size(globalFilter.RowCount)
            .Index(globalFilter.IndexName)
            );
            return response.Documents;
        }


        #endregion
    }

}

*/