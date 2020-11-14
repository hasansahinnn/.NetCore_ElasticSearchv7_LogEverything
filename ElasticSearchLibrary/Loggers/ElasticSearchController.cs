using ElasticSearchLibrary.ElasticSearchCore;
using ElasticSearchLibrary.Loggers.LogModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.Loggers
{ 
    [ApiController]
    [Route("Elastic/[controller]/[action]")]
    public class ElasticSearchController : Controller
    {
        private IElasticSearchManager elasticSearchService;
        public ElasticSearchController(IElasticSearchManager _elasticSearchService)
        {
            this.elasticSearchService = _elasticSearchService;
        }
        [HttpGet]
        public IActionResult ClearActivityLogs()
        {
            var result = elasticSearchService.ClearIndexData<ActivityLog>(IndexType.activity_log,new ActivityLog()); // Use lowerCase Index Name
            return Ok(result);
        }
    }
}
