using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static qbus2ha.Services.DiscoveryService;

namespace qbus2ha.Devices
{
    internal class Switch : Device
    {

        public Switch(string id)
            :base(id)
        {
        }

        public override string ComponentName => HAConstants.Components.Switch;

        public override void AddDeviceProperties(QbusFunctionBlock qbusFunctionBlock, JsonObject payload)
        {

            var payload_on = $$""" 
                { "id":"{{Id}}", "type": "state", "properties": {"value": true} }
            """;

            var payload_off = $$""" 
                { "id":"{{Id}}", "type": "state", "properties": {"value": false} }
            """;

            payload["pl_on"] = payload_on;
            payload["pl_off"] = payload_off;
            payload["val_tpl"] = "{{ value_json['properties']['value'] }}";
            payload["stat_on"] = true;
            payload["stat_off"] = false;
        }
    }
}
