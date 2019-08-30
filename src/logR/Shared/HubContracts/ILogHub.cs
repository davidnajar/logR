using logR.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace logR.Shared.HubContracts
{
    public interface ILogHub
    {
        Task NewLog(LogEvent log);
        Task NewLogSimple(string log);
    }
}
