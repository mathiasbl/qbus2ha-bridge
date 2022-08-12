using qbus2ha.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace qbus2ha.Devices
{
    internal abstract class Sensor : IDevice
    {
        public Sensor(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public string ComponentName => HAConstants.Components.Sensor;

        public virtual JsonObject GetConfigurationPayload(DiscoveryService.QbusFunctionBlock qbusFunctionBlock)
        {
            var baseTopic = $"~/{Id}";

            var uniqueId = qbusFunctionBlock.name.Replace("-", "_");
            var name = $"Qbus_{uniqueId.Replace(@"\", "_").Replace(".", "_")}";

            var payload = new JsonObject
            {
                ["name"] = name,
                ["stat_t"] = $"{baseTopic}/state",
                ["uniq_id"] = uniqueId,
            };

            return payload;
        }
    }
}
