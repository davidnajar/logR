using System;
using System.Collections.Generic;
using System.Text;

namespace logR.Shared.Gateways.Files
{
    public class FileGatewayConfiguration
    {
        public Guid Id { get; set; }
        public string Directory { get; set; }
        public string FileMask { get; set; }
        public FileFormat Format  { get; set; }
        public string FormatConfiguration { get; set; }

    }
}
