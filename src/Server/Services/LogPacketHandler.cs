using logR.Server.Services.Interfaces;
using logR.Shared;
using Networker.Common;
using Networker.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Services
{
    public class LogPacketHandler : PacketHandlerBase<LogEvent>
    {
        private readonly ILogProcessor _logProcessor;

        public LogPacketHandler(ILogProcessor logProcessor)
        {
            _logProcessor = logProcessor;
        }

        public override async Task Process(LogEvent packet, IPacketContext context)
        {
           await  _logProcessor.ProcessLog(packet);
        }
    }
}
