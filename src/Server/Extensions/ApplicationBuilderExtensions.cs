using logR.Server.Services;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLogServerReceiver(this IApplicationBuilder builder, IServiceProvider serviceProvider)
        {
            var service = builder.ApplicationServices.GetService(typeof(LogServerReceiver)) as LogServerReceiver;
            service.Start();
            return builder;
        }
    }
}
