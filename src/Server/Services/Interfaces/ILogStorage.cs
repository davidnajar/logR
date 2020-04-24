using logR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace logR.Server.Services.Interfaces
{
   public interface ILogStorage
    {
        Task StoreLog(LogEvent logEvent);
        Task<IEnumerable<LogEvent>> SearchLogs(Expression<Func<LogEvent, bool>> query);
    }
}
