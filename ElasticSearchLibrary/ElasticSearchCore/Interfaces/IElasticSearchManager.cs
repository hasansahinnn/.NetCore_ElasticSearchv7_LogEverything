using ElasticSearchLibrary.Loggers.LogModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.ElasticSearchCore
{
    public interface IElasticSearchManager
    {
        /// <summary>
        ///     Log Insert Function with String IndexType
        /// </summary>
        public Task CheckExistsAndInsert<T>(IndexType IndexType, T logs) where T : class;
        /// <summary>
        ///     Log Insert Function with String Index Name
        /// </summary>
        public Task CheckExistsAndInsert<T>(string IndexName, T logs) where T : class;
        /// <summary>
        ///     Clear Logs  with IndexType
        /// </summary>
        public Task ClearIndexData<T>(IndexType IndexType, T logs) where T : class, new();
        /// <summary>
        ///     Clear Logs  with string
        /// </summary>
        public Task ClearIndexData<T>(string indexName, T logs) where T : class, new();
        /// <summary>
        /// Delete Index
        /// </summary>
        public Task DeleteIndex(string indexName);
        /// <summary>
        /// Delete Index And Create Again
        /// </summary>
        public Task ReIndex<T>(string indexName, T logs) where T : class, new();

        public IReadOnlyCollection<ActivityLog> SearchActivityLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "", string httpMethod="",
            int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.activity_log, string indexNameStr="");

        public IReadOnlyCollection<ErrorLog> SearchErrorLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "",
        string method="",int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.error_log, string indexNameStr = "");

        public IReadOnlyCollection<ErrorLog> SearchChangeLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string className = "", ChangeState operation = ChangeState.Updated,
        int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.change_log, string indexNameStr = "");
    }
}
