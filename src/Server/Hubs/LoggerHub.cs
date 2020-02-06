using logR.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Hubs
{
    public class LoggerHub:Hub<ILoggerClient>
    {
    }

    public interface ILoggerClient
    {
        Task NewLogEvent(LogEvent logEvent);
    }
}
