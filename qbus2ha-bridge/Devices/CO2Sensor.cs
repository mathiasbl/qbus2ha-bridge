using qbus2ha.Services;
using System.Text.Json.Nodes;

namespace qbus2ha.Devices
{

    internal class CO2Sensor : Sensor
    {
        public CO2Sensor(string id) 
            : base(id)
        {
        }

        public override JsonObject GetConfigurationPayload(DiscoveryService.QbusFunctionBlock qbusFunctionBlock)
        {
            var payload =  base.GetConfigurationPayload(qbusFunctionBlock);

            payload["unit_of_meas"] = "ppm";
            payload["val_tpl"] = "{{ value_json['properties']['co2'] }}";
            payload["dev_cla"] = "co2";

            return payload;
        }
    }
}
