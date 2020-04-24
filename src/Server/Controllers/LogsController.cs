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
        private readonly ILogStorage _storage;

        public LogsController(ILogProcessor logProcessor, ILogStorage storage)
        {
            _logProcessor = logProcessor;
            _storage = storage;
        }
        [HttpPut]
        public void PutLog(LogEvent ect)
        {
            _logProcessor.ProcessLog(ect);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string filter)
        {
             filter = filter.ToUpper();
            return Ok(await _storage.SearchLogs(p => p.Message.ToUpper().Contains(filter)));
        }
    }
}
