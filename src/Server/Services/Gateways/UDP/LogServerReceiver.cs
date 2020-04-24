using logR.Shared;
using Microsoft.AspNetCore.Hosting.Server;
using Networker.Extensions.Json;
using Microsoft.Extensions.Logging;
using Networker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Networker.Client;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace logR.Server.Services
{
    public class LogServerReceiver
    {
        Networker.Server.Abstractions.IServer _server;
        public LogServerReceiver(IServiceCollection serviceCollection, IServiceProvider appServices)
        {
            LogPacketHandlerFactory.ServiceScopeFactory = appServices.GetService<IServiceScopeFactory>();
            _server = new ServerBuilder().SetServiceCollection(serviceCollection)
                  
                .UseTcp(1100)
                .UseUdp(1101)
                .UseJson()
                  .ConfigureLogging(loggingBuilder =>
                  {
                      loggingBuilder.AddConsole();
                      loggingBuilder.SetMinimumLevel(
                          LogLevel.Debug);
                  })
                .RegisterPacketHandler<LogEvent, LogPacketHandlerFactory>()
            
                .Build();

            //var client = new ClientBuilder().UseIp("127.0.0.1")
            //     //   .UseTcp(1100)
            //        .UseUdp(1101)
            //        .UseJson()
            //        .Build();
            //var log = new LogEvent()
            //{
            //    Logger = "test",
            //    Message = "Prueba de log",
            //    Severity = Severity.Debug,
            //    Vault = "EDS"
            //};
            
            //client.Connect();
            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        client.SendUdp(log);
            //        Thread.Sleep(1000);
            //    }
            //});
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
