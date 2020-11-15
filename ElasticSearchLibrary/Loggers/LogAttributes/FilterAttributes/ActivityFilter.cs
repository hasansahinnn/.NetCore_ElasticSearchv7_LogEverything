using ElasticSearchLibrary.ElasticSearchCore;
using ElasticSearchLibrary.ElasticSearchCore.LibraryConfiguration;
using ElasticSearchLibrary.Loggers.LogModels;
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
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.Loggers.LogAttributes.FilterAttributes
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
        private readonly IElasticSearchManager elasticSearchService;
        private readonly bool LogAllEndPoints;
        private StringBuilder requestParams=new StringBuilder();
        public ActivityFilter(IElasticSearchManager _elasticSearchService, IHostingEnvironment _hostingEnv, bool LogAllEndPoints)
        {
            this.LogAllEndPoints = LogAllEndPoints;
            this.elasticSearchService = _elasticSearchService;
            this.HostingEnvironment = _hostingEnv;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!HasNoLogAttribute(context) && (LogAllEndPoints || HasLogAttribute(context)))
            {
                var (controllerName, actionName, httpType) = context.GetControllerAndActionName();
             
                ActivityLog activity = new ActivityLog
                {
                    ControllerName = controllerName,
                    ActionName = actionName,
                    HttpType = httpType,
                    StatusCode= (context.Result as ObjectResult)?.StatusCode??0,
                    UserId = context.HttpContext.GetAuthUser(), // UserId
                    Params = requestParams.ToString(),
                    IPAddress = this.HostingEnvironment.IsDevelopment() ? "" : context.HttpContext.Connection.RemoteIpAddress.ToString(), // Get User Ip Address
                    DateCreated = DateTime.Now
                };
                elasticSearchService.CheckExistsAndInsert<ActivityLog>(IndexType.activity_log, activity); // Send to Elastic
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments) // Get Request Json Body
            {
                requestParams.Append(arg.Key.ToString() + ":" + Newtonsoft.Json.JsonConvert.SerializeObject(arg.Value) + "\n");
            }
        }

        public bool HasLogAttribute(FilterContext context)  //Check LogEndpointAttribute for action
        {
            return ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.CustomAttributes.Any(filterDescriptors => filterDescriptors.AttributeType == typeof(LogEndpointAttribute));
        }
        public bool HasNoLogAttribute(FilterContext context)  //Check NoLogEndpointAttribute for action
        {
            return ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.CustomAttributes.Any(filterDescriptors => filterDescriptors.AttributeType == typeof(NoLogEndpointAttribute));
        }
    }

}
