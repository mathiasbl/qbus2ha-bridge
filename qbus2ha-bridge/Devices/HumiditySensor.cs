using qbus2ha.Services;
using System.Text.Json.Nodes;

namespace qbus2ha.Devices
{
    internal class HumiditySensor : Sensor
    {
        public HumiditySensor(string id)
            : base(id)
        {
        }

        public override JsonObject GetConfigurationPayload(DiscoveryService.QbusFunctionBlock qbusFunctionBlock)
        {
            var payload = base.GetConfigurationPayload(qbusFunctionBlock);

            payload["unit_of_meas"] = "%";
            payload["val_tpl"] = "{{ value_json['properties']['humidity'] }}";
            payload["dev_cla"] = "humidity";

            return payload;
        }
    }
}
