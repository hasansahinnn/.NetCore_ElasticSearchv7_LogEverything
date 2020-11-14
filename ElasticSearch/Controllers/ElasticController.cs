using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchLibrary;
using ElasticSearchLibrary.ElasticSearchCore;
using ElasticSearchLibrary.Loggers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElasticSearch.Controllers
{
    [Route("api/[controller]/[action]")]
    [SwaggerTag("ElasticSearch Manager")]
    public class ElasticController : ElasticSearchController
    {
        public ElasticController(IElasticSearchManager _elasticSearchService) : base(_elasticSearchService) { }
    }
}
