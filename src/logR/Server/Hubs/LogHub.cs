using logR.Shared.HubContracts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logR.Server.Hubs
{
    public class LogHub :Hub<ILogHub>
    {
        public override Task OnConnectedAsync()
        {
            Clients.All.NewLog(new Shared.Models.LogEvent()
            {

                Message = $"{Context.ConnectionId} connected "
            });

            Clients.All.NewLogSimple($"{Context.ConnectionId} connected ");

            return base.OnConnectedAsync();
        }
    }
   
}
