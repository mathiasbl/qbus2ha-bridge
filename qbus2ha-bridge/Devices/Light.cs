using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static qbus2ha.Services.DiscoveryService;

namespace qbus2ha.Devices
{
    internal class Light: Device
    {

        public Light(string id)
            : base(id)
        {
        }

        public override string ComponentName => HAConstants.Components.Light;

        public override void AddDeviceProperties(QbusFunctionBlock qbusFunctionBlock, JsonObject payload)
        {
            payload["cmd_on_tpl"] = $$""" 
            {% - if brightness is defined -%}
                     { "id":"{{Id}}","type":"state","properties":{ "value":{ { brightness | float | multiply(0.39215686) | round(0)} } } }
                {% - else -%}
                     { "id":"{{Id}}","type":"state","properties":{ "value":100} }
                {% -endif -%}
            }  
            """;

            payload["cmd_off_tpl"] = $$"""
                { "id":"{{Id}}", "type": "state", "properties": {"value": 0} }
            """;

            payload["schema"] = "template";
            payload["bri_tpl"] = "{{ value_json.properties.value | float | multiply(2.55) | round(0) }}";
            payload["stat_tpl"] = "{{% if value_json.properties.value > 0 %} on {% else %} off {% endif %}}";
        }
    }
}
