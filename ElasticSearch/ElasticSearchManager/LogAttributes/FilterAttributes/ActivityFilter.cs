using ElasticSearch.Configuration;
using ElasticSearch.ElasticSearchManager;
using ElasticSearch.ElasticSearchManager.LogAttributes.FilterAttributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.ElasticSearchManager.LogAttributes.FilterAttributes
{
    public class ActivityFilterAttribute : TypeFilterAttribute
    {
        public ActivityFilterAttribute(bool LogAllEndPoints=false) : base(typeof(ActivityFilter)) // Log All EndPoints || use [LogAttribute] attribute
        {
            base.Arguments = new object[] { LogAllEndPoints };
        }
    }
    public class ActivityFilter : IActionFilter
    {
        protected readonly IHostingEnvironment HostingEnvironment;
        private readonly IElasticSearchManager<ActivityLog> elasticSearchService;
        private readonly bool LogAllEndPoints;
        public ActivityFilter(IElasticSearchManager<ActivityLog> _elasticSearchService, IHostingEnvironment _hostingEnv, bool LogAllEndPoints)
        {
            this.LogAllEndPoints = LogAllEndPoints;
            this.elasticSearchService = _elasticSearchService;
            this.HostingEnvironment = _hostingEnv;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        
        }
        private ConcurrentDictionary<string, string> requests = new ConcurrentDictionary<string, string>();

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!HasNoLogAttribute(context) && ( LogAllEndPoints || HasLogAttribute(context)))
            {
                var (controllerName, actionName) = context.GetControllerAndActionName();
                StringBuilder sb = new StringBuilder();
                foreach (var arg in context.ActionArguments) // Get Request Json Body
                {

                    sb.Append(arg.Key.ToString() + ":" + Newtonsoft.Json.JsonConvert.SerializeObject(arg.Value) + "\n");

                }
                requests.TryAdd(context.HttpContext.TraceIdentifier, sb.ToString());

                ActivityLog activity = new ActivityLog
                {
                    ControllerName = controllerName,
                    ActionName = actionName,
                    UserId = context.HttpContext.GetAuthUser(), // UserId
                    Params = sb.ToString(),
                    IPAddress = this.HostingEnvironment.IsDevelopment() ? "" : context.HttpContext.Connection.RemoteIpAddress.ToString(), // Get User Ip Address
                    DateCreated = DateTime.Now
                };
                elasticSearchService.CheckExistsAndInsert(IndexType.activity_log, activity); // Send to Elastic
            }
        }

        public bool HasLogAttribute(FilterContext context)
        {
            return ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.CustomAttributes.Any(filterDescriptors => filterDescriptors.AttributeType == typeof(LogEndpointAttribute));
        }
        public bool HasNoLogAttribute(FilterContext context)
        {
            return ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.CustomAttributes.Any(filterDescriptors => filterDescriptors.AttributeType == typeof(NotLogEndpointAttribute));
        }
    }

}
