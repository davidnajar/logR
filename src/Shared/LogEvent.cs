using System;
using System.Collections.Generic;
using System.Text;

namespace logR.Shared
{
   public class LogEvent
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public string Logger { get; set; }
        public string Vault { get; set; }
        public string Exception { get; set; }
        public Severity Severity { get; set; }
    }
}
