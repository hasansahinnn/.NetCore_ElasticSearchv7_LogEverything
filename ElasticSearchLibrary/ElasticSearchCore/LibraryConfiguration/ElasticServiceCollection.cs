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
        public static IServiceCollection AddElasticServices(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IElasticConnectionSettings), typeof(ElasticConnectionSettings));
            services.AddSingleton(typeof(IElasticClientProvider), typeof(ElasticClientProvider));
            services.AddSingleton(typeof(IElasticSearchManager), typeof(ElasticSearchManager));
            services.AddSingleton(typeof(IDatabaseLogger), typeof(DatabaseLogger));
            return services;
        }

        public static IApplicationBuilder AddElasticErrorHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                IElasticSearchManager _elasticSearchService;
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    _elasticSearchService = scope.ServiceProvider.GetRequiredService<IElasticSearchManager>(); // Get ElasticSearchManager Service Instance
                }
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>(); // Catches Exception Detail
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
            return app;
        }
    }
}
