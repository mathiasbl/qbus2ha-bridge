using qbus2ha.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace qbus2ha.Devices
{
    internal class Climate : Device
    {
        public Climate(string id)
            : base(id)
        {
        }

        public override string ComponentName => HAConstants.Components.Climate;

        public override void AddDeviceProperties(DiscoveryService.QbusFunctionBlock qbusFunctionBlock, JsonObject payload)
        {
            payload["curr_temp_t"] = payload["stat_t"];
            payload["curr_temp_tpl"] = "{{ value_json['properties']['currTemp'] }}";

            payload["modes"] = new JsonArray("heat", "cool", "off", "auto");
            payload["precision"] = 0.5;

            var preset_mode_command_template = $$$""" 
                { "id":"{{{Id}}}", "type": "state", "properties": { "currRegime": {{value}} } }
            """;
         
            payload["pr_mode_cmd_t"] = payload["cmd_t"];
            payload["pr_mode_cmd_tpl"] = preset_mode_command_template;
            payload["pr_mode_stat_t"] = payload["stat_t"];
            payload["pr_mode_val_tpl"] = "{{ value_json['properties']['currRegime'] }}";
            payload["pr_modes"] = new JsonArray("MANUEEL", "VORST", "ECONOMY", "COMFORT", "NACHT");

            var temperature_command_template = $$$""" 
                { "id":"{{{Id}}}", "type": "state", "properties": { "setTemp": {{value}} } }
            """;

            payload["temp_stat_t"] = payload["stat_t"];
            payload["temp_cmd_t"] = payload["cmd_t"];
            payload["temp_cmd_tpl"] = temperature_command_template;
            payload["temp_stat_tpl"] = "{{ value_json['properties']['setTemp'] }}";

            payload["temp_unit"] = "C";
            payload["temp_step"] = 0.5;

            payload.Remove("cmd_t");
            payload.Remove("stat_t");
        }
    }
}
