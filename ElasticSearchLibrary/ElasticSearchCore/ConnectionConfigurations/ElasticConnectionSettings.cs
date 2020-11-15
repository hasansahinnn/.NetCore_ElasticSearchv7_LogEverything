using ElasticSearchLibrary.ElasticSearchCore.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.ElasticSearchCore
{
    public class ElasticConnectionSettings: IElasticConnectionSettings
    {
        public IConfiguration Configuration { get; }
        public ElasticConnectionSettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string ElasticSearchHost { get { return Configuration.GetSection("ElasticConnectionSettings:ConnectionString:HostUrls").Value; } } // Get HostUrl
        public string UserName { get { return Configuration.GetSection("ElasticConnectionSettings:ConnectionString:UserName").Value; } } // Get UserName
        public string PassWord { get { return Configuration.GetSection("ElasticConnectionSettings:ConnectionString:Password").Value; } } // Get PassWord

    }
}
