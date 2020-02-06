﻿using logR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Services.Interfaces
{
 public   interface ILogBroadcaster
    {
        Task BroadcastToClients(LogEvent logEvent);
    }
}