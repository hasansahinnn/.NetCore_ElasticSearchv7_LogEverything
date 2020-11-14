using ElasticSearchLibrary.Loggers.LogModels;
using ElasticSearchLibrary.Loggers.SearchModels;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.ElasticSearchCore
{
    public interface IElasticSearchManager
    {


        /// <summary>
        ///     Create New Index merged with alias
        /// </summary>
        public Task<IBulkAliasResponse> CreateIndexAsync(string aliasName, string indexName);
        /// <summary>
        ///     Create New Index and Alias
        /// </summary>
        Task<IBulkAliasResponse> CreateIndexAndAliasAsync(string indexName);
        /// <summary>
        ///     Create Alias and optional merge Index 
        /// </summary>
        Task<IBulkAliasResponse> CreateAliasAsync(string aliasName, string? IndexName);




        /// <summary>
        ///     Log Insert Function with String IndexType
        /// </summary>
        public Task<IIndexResponse> CheckExistsAndInsert<T>(IndexType IndexType, T logs) where T : class;
        /// <summary>
        ///     Log Insert Function with String Index Name
        /// </summary>
        public Task<IIndexResponse> CheckExistsAndInsert<T>(string IndexName, T logs) where T : class;



        /// <summary>
        ///     Clear Logs  with IndexType
        /// </summary>
        public Task<IDeleteByQueryResponse> ClearIndexData(IndexType IndexType);
        /// <summary>
        ///     Clear Logs  with string
        /// </summary>
        public Task<IDeleteByQueryResponse> ClearIndexData(string indexName);





        /// <summary>
        /// Delete Index
        /// </summary>
        public Task<IDeleteIndexResponse> DeleteIndex(string indexName);
        /// <summary>
        /// Delete Alias. If you want to delete Indexs too set Delete paramater true. 
        /// </summary>
        public Task<IBulkAliasResponse> DeleteAlias(string aliasName,bool DeleteAllIndexs);




        /// <summary>
        ///     Get Alias Name using IndexName
        /// </summary>
        Task<IEnumerable<AliasDefinition>> GetAliasByIndex(string indexName);

        /// <summary>
        ///     Get All Index using Alias Name
        /// </summary>
        Task<IEnumerable<string>> GetAllIndexByAlias(string aliasName);


        /// <summary>
        /// Delete Index And Create Again
        /// </summary>
        public Task<IReindexOnServerResponse> ReIndex(string sourceIndexName, string destinationIndexName);


        public Task<IReadOnlyCollection<ActivityLog>> SearchActivityLogs(ActivitySearchModel activityFilter);

        public Task<IReadOnlyCollection<ErrorLog>> SearchErrorLogs(ErrorSearchModel errorFilter);

        public Task<IReadOnlyCollection<EntityLog>> SearchEntityLogs(EntitySearchModel entityFilter);
        public Task<IReadOnlyCollection<T>> SearchIndexLogs<T>(GlobalSearchModel globalFilter) where T : class;
    }
}
