using logR.Shared;
using Microsoft.Extensions.DependencyInjection;
using Networker.Common;
using Networker.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Services
{
    public class LogPacketHandlerFactory : PacketHandlerBase<LogEvent>
    {
        public static  IServiceScopeFactory ServiceScopeFactory { get; set; }
        

        public LogPacketHandlerFactory()
        {
          
           
        }
        public override Task Process(LogEvent packet, IPacketContext context)
        {
            
            using (var scope = ServiceScopeFactory.CreateScope())
            {
              return  scope.ServiceProvider.GetService<LogPacketHandler>().Process(packet, context);
            }
        }
    }
}
