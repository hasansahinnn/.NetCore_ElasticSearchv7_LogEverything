using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.ElasticSearchManager
{
    public interface IElasticSearchManager<T> where T: class
    {
        public void CheckExistsAndInsert(IndexType IndexType, T logs);

        public IReadOnlyCollection<ActivityLog> SearchActivityLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "",
            int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.activity_log);

        public IReadOnlyCollection<ErrorLog> SearchErrorLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string controller = "", string action = "",
        string method="",int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.error_log);

        public IReadOnlyCollection<ErrorLog> SearchChangeLogs(int? userId, int? errorCode, DateTime? beginDate, DateTime? endDate, string className = "", ChangeState operation = ChangeState.Updated,
        int? page = 0, int? rowCount = 10, IndexType indexName = IndexType.change_log);
    }
}
