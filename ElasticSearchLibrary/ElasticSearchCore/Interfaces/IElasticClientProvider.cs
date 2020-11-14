using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.ElasticSearchCore.Interfaces
{
    public interface IElasticClientProvider
    {
         ElasticClient CreateClient();
         ElasticClient CreateClientWithIndex(string defaultIndex); 
         ElasticClient ElasticClient { get; }

         string ElasticSearchHost { get; set; }
    }
}
