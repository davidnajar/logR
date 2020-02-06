using logR.Server.Hubs;
using logR.Server.Services.Interfaces;
using logR.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController:ControllerBase
    {
        private readonly ILogProcessor _logProcessor;

        public LogsController(ILogProcessor logProcessor)
        {
            _logProcessor = logProcessor;
        }
        [HttpPut]
        public void PutLog(LogEvent ect)
        {
            _logProcessor.ProcessLog(ect);
        }
    }
}
