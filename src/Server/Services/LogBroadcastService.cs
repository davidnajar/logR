using logR.Server.Hubs;
using logR.Server.Services.Interfaces;
using logR.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Services
{
    public class LogBroadcastService : ILogBroadcaster
    {
        private readonly IHubContext<LoggerHub, ILoggerClient> _hubContext;

        public LogBroadcastService(IHubContext<LoggerHub, ILoggerClient> hubContext)
        {
            _hubContext = hubContext;
        }
        public Task BroadcastToClients(LogEvent logEvent)
        {
            return _hubContext.Clients.All.NewLogEvent(logEvent);
        }
    }
}
