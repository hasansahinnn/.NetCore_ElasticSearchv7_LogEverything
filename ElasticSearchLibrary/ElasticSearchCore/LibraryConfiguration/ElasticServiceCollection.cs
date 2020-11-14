using ElasticSearchLibrary.ElasticSearchCore;
using ElasticSearchLibrary.ElasticSearchCore.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using System.Reflection;
using ElasticSearchLibrary.Loggers.DBEntityLogger;
using ElasticSearchLibrary.Loggers.LogModels;

namespace ElasticSearchLibrary.ElasticSearchCore.LibraryConfiguration
{
    public static class ElasticServiceCollection
    {
        //public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        public static IServiceCollection AddElasticServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped(typeof(IElasticConnectionSettings), typeof(ElasticConnectionSettings));
            services.AddScoped(typeof(IElasticClientProvider), typeof(ElasticClientProvider));
            services.AddScoped(typeof(IElasticSearchManager), typeof(ElasticSearchManager));
            services.AddScoped(typeof(IDatabaseLogger), typeof(DatabaseLogger));
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetService<IElasticSearchManager>();
            return services;
        }

        public static IApplicationBuilder AddElasticErrorHandler(this IApplicationBuilder app,IConfiguration configuration)
        {
           
            return app.UseExceptionHandler(a => a.Run(async context => 
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            var _elasticSearchService = app.ApplicationServices.GetService<ElasticSearchManager>();
                ErrorLog error = new ErrorLog
                {
                    UserId = context.GetAuthUser(),
                    Path = exceptionHandlerPathFeature.Path,
                    StatusCode = context.Response.StatusCode,
                    Method = context.Request.Method,
                    ErrorMessage = exceptionHandlerPathFeature.Error.Message
                };
                _elasticSearchService.CheckExistsAndInsert<ErrorLog>(IndexType.error_log, error); // Insert Error to Elastic

                context.Response.ContentType = "application/json";

                // --- Return ErrorLog
                //var result = JsonConvert.SerializeObject(error);
                //await context.Response.WriteAsync(result);
                // --- Return ErrorLog

                var result = JsonConvert.SerializeObject(new { error = exceptionHandlerPathFeature.Error.Message });
                await context.Response.WriteAsync(result);
            }));
        }
    }
}
