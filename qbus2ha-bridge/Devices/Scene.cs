using qbus2ha.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace qbus2ha.Devices
{
    internal class Scene : Device
    {
        public override string ComponentName => HAConstants.Components.Scene;

        public Scene(string id) 
            : base(id)
        {
        }

        public override void AddDeviceProperties(DiscoveryService.QbusFunctionBlock qbusFunctionBlock, JsonObject payload)
        {
            var payload_on = $$""" 
                { "id":"{{Id}}", "type": "action", "action": "active", "properties": { } }
            """;

            payload["pl_on"] = payload_on;

            payload.Remove("stat_t"); // state not supported bij scene
        }
    }
}
