using ElasticSearchLibrary.ElasticSearchCore.Interfaces;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.ElasticSearchCore
{
    public class ElasticClientProvider:IElasticClientProvider
    {
        protected readonly IElasticConnectionSettings elasticSearchConfigration;
        public ElasticClientProvider(IElasticConnectionSettings _elasticSearchConfigration)
        {
            elasticSearchConfigration = _elasticSearchConfigration;
            ElasticClient = CreateClient();
        }
       
        public ElasticClient CreateClient()
        {
            var connectionSettings = new ConnectionSettings(new Uri(elasticSearchConfigration.ElasticSearchHost))
                .DisablePing()
                .DisableDirectStreaming(true)
                .SniffOnStartup(false)
                .SniffOnConnectionFault(false);
            if (!string.IsNullOrEmpty(elasticSearchConfigration.UserName) && !string.IsNullOrEmpty(elasticSearchConfigration.PassWord))
                connectionSettings.BasicAuthentication(elasticSearchConfigration.UserName, elasticSearchConfigration.PassWord);

            return new ElasticClient(connectionSettings);
        }

        public ElasticClient CreateClientWithIndex(string defaultIndex)
        {
            var connectionSettings = new ConnectionSettings(new Uri(ElasticSearchHost))
                .DisablePing()
                .SniffOnStartup(false)
                .SniffOnConnectionFault(false)
                .DefaultIndex(defaultIndex);
            if (!string.IsNullOrEmpty(elasticSearchConfigration.UserName) && !string.IsNullOrEmpty(elasticSearchConfigration.PassWord))
                connectionSettings.BasicAuthentication(elasticSearchConfigration.UserName, elasticSearchConfigration.PassWord);

            return new ElasticClient(connectionSettings);
        }

        public ElasticClient ElasticClient { get; }

        public string ElasticSearchHost { get; set; }


    }
   
   
}
