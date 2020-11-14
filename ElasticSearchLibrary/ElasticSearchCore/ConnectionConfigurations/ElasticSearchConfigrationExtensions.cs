using ElasticSearchLibrary.ElasticSearchCore.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.ElasticSearchCore
{
    public static class ElasticSearchConfigrationExtensions
    {
        public static IElasticConnectionSettings GetElasticConfiguration(this IConfiguration configuration)
        {
            return configuration.Get<IElasticConnectionSettings>();
        }
    }
}
