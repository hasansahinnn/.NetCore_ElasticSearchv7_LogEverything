using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchLibrary.Loggers.LogAttributes.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NoLogEndpointAttribute : Attribute
    {
    }
}
