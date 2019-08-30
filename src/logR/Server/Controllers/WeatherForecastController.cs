using logR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using logR.Server.Hubs;
using logR.Shared.HubContracts;
using logR.Shared.Models;

namespace logR.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
       
        private readonly ILogger<LogController> _logger;
        private readonly IHubContext<LogHub, ILogHub> _logHubContext;

        public LogController(ILogger<LogController> logger, IHubContext<LogHub, ILogHub> logHubContext)
        {
            _logger = logger;
            _logHubContext = logHubContext;
        }
        [HttpPost]
        public async Task<IActionResult> StoreLog(LogEvent logEvent)
        {

            await _logHubContext.Clients.All.NewLog(logEvent);
            await _logHubContext.Clients.All.NewLogSimple(logEvent.Message);

            return Ok();
        }
        [HttpGet]
        public  IActionResult Get()
        {



            return Ok(new LogEvent(){ Message = "This is a log" });
        }
    }
}
