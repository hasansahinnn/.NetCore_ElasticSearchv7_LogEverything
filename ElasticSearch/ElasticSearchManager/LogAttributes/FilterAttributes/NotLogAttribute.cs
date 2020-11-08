using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.ElasticSearchManager.LogAttributes.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NotLogEndpointAttribute : Attribute
    {
    }
}
