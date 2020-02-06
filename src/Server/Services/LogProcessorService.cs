using logR.Server.Services.Interfaces;
using logR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Services
{
    public class LogProcessorService : ILogProcessor
    {
        private readonly ILogBroadcaster _broadcastService;
        private readonly ILogStorage _storageService;

        public LogProcessorService(ILogStorage storageService, ILogBroadcaster broadcastService)
        {
            _broadcastService = broadcastService;
            _storageService = storageService;
        }
        public async Task ProcessLog(LogEvent logEvent)
        {
            EnsureDefaults(logEvent);
            await _broadcastService.BroadcastToClients(logEvent);
            await _storageService.StoreLog(logEvent);
        }

        void EnsureDefaults(LogEvent logEvent)
        {
            logEvent.Id = Guid.NewGuid();
            if (logEvent.Time == default)
            {
                logEvent.Time = DateTime.Now;
            }
        }
    }
}
