using System;
using System.Collections.Generic;
using System.Text;

namespace logR.Shared.Gateways
{
    public class GatewayDefinition
    {
        public Guid GatewayId { get; set; }
        public string VaultName { get; set; }
        public string GatewayType { get; set; }
    }
}
