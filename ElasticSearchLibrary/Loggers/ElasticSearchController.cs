using ElasticSearchLibrary.ElasticSearchCore;
using ElasticSearchLibrary.Loggers.LogModels;
using ElasticSearchLibrary.Loggers.SearchModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.Loggers
{ 
    //[ApiController]
    //[Route("Elastic/[controller]/[action]")]
    public class ElasticSearchController : Controller
    {
        private IElasticSearchManager elasticSearchService;
        public ElasticSearchController(IElasticSearchManager _elasticSearchService)
        {
            this.elasticSearchService = _elasticSearchService;
        }

        #region Default Logs

        /// <summary>
        ///  Clear all Activity Logs 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ClearActivityLogs()
        {
            var result = await elasticSearchService.ClearIndexData(IndexType.activity_log); // Use lowerCase Index Name
            return Ok(result);
        }
        /// <summary>
        ///  Clear all Error Logs 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ClearErrorLogs()
        {
            var result = await elasticSearchService.ClearIndexData(IndexType.error_log); // Use lowerCase Index Name
            return Ok(result);
        }
        /// <summary>
        ///  Clear all DB Changes Logs 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ClearEntityChangesLogs()
        {
            var result = await elasticSearchService.ClearIndexData(IndexType.entity_log); // Use lowerCase Index Name
            return Ok(result);
        }

        /// <summary>
        ///  Delete Index Documents
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteIndexDocuments(string indexName)
        {
            var result = await elasticSearchService.ClearIndexData(indexName); // Use lowerCase Index Name
            return Ok(result);
        }


        #endregion


        #region Get Endpoints

        /// <summary>
        ///  Get Alias Name using Index Name
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAliasName(string indexName)
        {
            var result = await elasticSearchService.GetAliasByIndex(indexName); 
            return Ok(result);
        }

        /// <summary>
        ///   Get All Index using Alias Name
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllIndexByAlias(string aliasName)
        {
            var result = await elasticSearchService.GetAllIndexByAlias(aliasName); 
            return Ok(result);
        }

        #endregion


        #region Create Endpoints

        /// <summary>
        ///     Create New Alias (Index Optional for merge alias to index)   // Use lowerCase Index Name
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAlias(string aliasName, string? IndexName)
        {
            var result = await elasticSearchService.CreateAliasAsync(aliasName, IndexName);
            return Ok(result);
        }
        /// <summary>
        ///     Create Index 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateIndexAndAlias(string IndexName) 
        {
            var result = await elasticSearchService.CreateIndexAndAliasAsync( IndexName);  // Use lowerCase Index Name
            return Ok(result);
        }
        /// <summary>
        ///     Create Index merged to Alias
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateIndex(string aliasName, string IndexName)
        {
            var result = await elasticSearchService.CreateIndexAsync(aliasName,IndexName);
            return Ok(result);
        }

        #endregion


        #region Delete Endpoints

        /// <summary>
        ///  Delete Index
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteIndex(string indexName)
        {
            var result = await elasticSearchService.DeleteIndex(indexName); 
            return Ok(result);
        }
        /// <summary>
        ///  Delete Alias. If you want to delete Indexs too set Delete paramater true. 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteAlias(string aliasName, bool DeleteAllIndexs)
        {
            var result = await elasticSearchService.DeleteAlias(aliasName, DeleteAllIndexs);
            return Ok(result);
        }


        /// <summary>
        ///     Delete Index Logged Data
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ClearIndexData(string indexName)
        {
            var result = await elasticSearchService.ClearIndexData(indexName);
            return Ok(result);
        }

        #endregion


        #region Insert Endpoints

        /// <summary>
        ///     Insert Log for your specific Models
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertLogForModel(string indexName, ErrorLog logdata)
        {
            var result = await elasticSearchService.CheckExistsAndInsert<ErrorLog>(indexName, logdata); 
            return Ok(result);
        }

        #endregion


        #region SearchEndPoints

        /// <summary>
        ///     Search Activity Logs
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SearchActivityLogs(ActivitySearchModel activityFilter)
        {
            var result = await elasticSearchService.SearchActivityLogs(activityFilter);
            return Ok(result);
        }

        /// <summary>
        ///     Search Activity Logs
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SearchErrorLogs(ErrorSearchModel errorFilter)
        {
            var result = await elasticSearchService.SearchErrorLogs(errorFilter);
            return Ok(result);
        }

        /// <summary>
        ///     Search Activity Logs
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SearchEntityLogs(EntitySearchModel entityFilter)
        {
            var result = await elasticSearchService.SearchEntityLogs(entityFilter);
            return Ok(result);
        }

        /// <summary>
        ///     Search Your Custom Logs
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SearchCustomLogs(GlobalSearchModel customSearch)
        {
            var result = await elasticSearchService.SearchIndexLogs<dynamic>(customSearch);
            return Ok(result);
        }


        #endregion



    }
}
