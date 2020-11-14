using ElasticSearchLibrary.ElasticSearchCore;
using ElasticSearchLibrary.Loggers.LogModels;
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


        #region Get Endpoints

        /// <summary>
        ///  Get Alias Name using Index Name
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAliasName(string indexName)
        {
            var result = await elasticSearchService.GetAliasByIndex(indexName); // Use lowerCase Index Name
            return Ok(result);
        }

        #endregion

        #region Create Endpoints

        /// <summary>
        ///     Create New Alias (Index Optional for merge alias to index)
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
            var result = await elasticSearchService.CreateIndexAndAliasAsync( IndexName);
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

        #endregion

    }
}
