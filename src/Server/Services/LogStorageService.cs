using logR.Server.Services.Interfaces;
using logR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Services
{
    public class LogStorageService : ILogStorage
    {
        public Task<IEnumerable<LogEvent>> SearchLogs(Predicate<LogEvent> query)
        {
            return Task.FromResult(default(IEnumerable<LogEvent>));
        }

        public Task StoreLog(LogEvent logEvent)
        {
            return Task.CompletedTask;
        }
    }
}
