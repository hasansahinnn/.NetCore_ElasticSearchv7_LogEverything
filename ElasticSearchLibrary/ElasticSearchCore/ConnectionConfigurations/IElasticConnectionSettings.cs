using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.ElasticSearchCore.Interfaces
{
    public interface IElasticConnectionSettings
    {
        string ElasticSearchHost { get; }
        string UserName { get; }
        string PassWord { get; }
    }
}
