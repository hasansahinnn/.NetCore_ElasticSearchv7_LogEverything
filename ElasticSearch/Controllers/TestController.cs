﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Configuration;
using ElasticSearch.Models;
using ElasticSearchLibrary.ElasticSearchCore;
using ElasticSearchLibrary.Loggers.LogAttributes.FilterAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace ElasticSearch.Controllers
{
    [ApiController]
    [Authorize]
    [ActivityFilter(LogAllEndPoints: false)]  
    // [ServiceFilter(typeof(AnotherFilterClass))]  You can use multiple filter 
    [Route("api/[controller]/[action]")]

    public class TestController : Controller
    {
        private readonly ProductContext dbContext;
        private IElasticSearchManager elasticSearchService;

        public TestController(ProductContext dbContext, IElasticSearchManager _elasticSearchService)
        {
            this.elasticSearchService = _elasticSearchService;
            this.dbContext = dbContext;
        }
        /// <summary>
        ///     Get Jwt Token for User ID:5
        /// </summary>
        [LogEndpoint] // if LogAllEndPoints:false -> Use [LogEndpoint] for log specific endpoints.
        [AllowAnonymous]
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Security" }, Summary = "Get Token for User Id 5")]
        public async Task<IActionResult> GetToken()
        {
            return Ok(new { token=$"Bearer {JwtTokenService.GenerateJSONWebToken(5)}" });
        }
        /// <summary>
        ///     Get Products
        /// </summary>
        [NoLogEndpoint] // if LogAllEndPoints:true -> Use [NoLogEndpoint] for remove logging to specific endpoints.
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "CRUD" }, Summary = "Get All Products. (Authorized)")]
        public IActionResult Get()
        {
             return Ok(dbContext.Products.ToList());
        }
        [HttpPost]
        [LogEndpoint]
        [SwaggerOperation(Tags = new[] { "CRUD" }, Summary = "Create new Product.  (Authorized)",Description = "Don't fill ID paramater")]
        public IActionResult Create(Product product)
        {
            dbContext.Products.Add(product); dbContext.SaveChanges();
            return Ok(product.ID);
        }
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "CRUD" }, Summary = "Update Product.  (Authorized)")]
        public IActionResult Update(Product product)
        {
            var updateProduct = dbContext.Products.FirstOrDefault(prod => prod.ID == product.ID);
            if (updateProduct == null) return Ok(false);
            dbContext.Entry(updateProduct).CurrentValues.SetValues(product);
            return Ok(dbContext.SaveChanges()>0);
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "CRUD" }, Summary = "Delete Product.  (Authorized)")]
        public IActionResult Delete(int productId)
        {
            var updateProduct = dbContext.Products.FirstOrDefault(prod => prod.ID == productId);
            if (updateProduct == null) return Ok(false);
            return Ok(dbContext.Remove(updateProduct));
        }

        [LogEndpoint]
        [AllowAnonymous]
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Error" }, Summary = "Error Log Test ")]
        public async Task<IActionResult> GetError()
        {
            throw new Exception("test error");
        }


        [AllowAnonymous]
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "SpecificLogTest" }, Summary = "Log Your Specific Models to Elastic.")]
        public IActionResult SpecialLog()
        {
            MyTestModel testData = new MyTestModel(1, "Test");
            elasticSearchService.CheckExistsAndInsert<MyTestModel>("test_model", testData);
            return Ok(true);
        }

    
    }
}
