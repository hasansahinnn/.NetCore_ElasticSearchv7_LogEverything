using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchLibrary.Loggers.DBEntityLogger
{
    public interface IDatabaseLogger
    {
        void LogChanges(ChangeTracker changes);
    }
}
