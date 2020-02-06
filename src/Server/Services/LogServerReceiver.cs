using logR.Shared;
using Microsoft.AspNetCore.Hosting.Server;
using Networker.Extensions.Json;
using Networker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Services
{
    public class LogServerReceiver
    {
        Networker.Server.Abstractions.IServer _server;
        public LogServerReceiver()
        {
             _server = new ServerBuilder()
                .UseTcp(1100)
                .UseUdp(1101)
                .UseJson()
                .RegisterPacketHandler<LogEvent, LogPacketHandler>().Build();
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}
