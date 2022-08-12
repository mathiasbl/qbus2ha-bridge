using qbus2ha.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace qbus2ha.Devices
{
    internal class Cover : Device
    {

        public Cover(string id) 
            : base(id)
        {
        }

        public override string ComponentName => HAConstants.Components.Cover;

        public override void AddDeviceProperties(DiscoveryService.QbusFunctionBlock qbusFunctionBlock, JsonObject payload)
        {
            payload["retain"] = true;

            if (qbusFunctionBlock.properties.Contains("up")) // is shutter relay
            {
                payload["pl_cls"] = $$"""
                    { "id":"{{Id}}", "type": "state", "properties": {"state": "down"} }
                """;

                payload["pl_open"] = $$"""
                    { "id":"{{Id}}", "type": "state", "properties": {"state": "up"} }
                """;

                payload["pl_stop"] = $$"""
                    { "id":"{{Id}}", "type": "state", "properties": {"state": "stop"} }
                """;

                payload["stat_opening"] = "up";
                payload["stat_closing"] = "down";
                payload["stat_stopped"] = "stop";
                payload["val_tpl"] = "{{ value_json['properties']['state'] }}";
            }
            else
            {
                payload["pl_cls"] = $$$"""
                    { "id":"{{{Id}}}", "type": "state", "properties": { "shutterPosition": 0, "slatPosition": 0 } }
                """;

                payload["pl_open"] = $$$"""
                    { "id":"{{{Id}}}", "type": "state", "properties": { "shutterPosition": 100, "slatPosition": 100 } }
                """;

                payload["position_topic"] = payload["stat_t"];
                payload["set_position_topic "] = payload["cmd_t"];
                payload["set_position_template"] = $$$"""
                    { "id":"{{{Id}}}", "type": "state", "properties": { "shutterPosition": {{position}} } }
                """;
          
                payload["tilt_command_topic"] = payload["cmd_t"];
                payload["tilt_command_template"] = $$$"""
                    { "id":"{{{Id}}}", "type": "state", "properties": { "slatPosition": {{position}} } }
                """;
  
                payload["position_template"] = $$$"""
                    {% set shutterPosition = state_attr(entity_id,"current_position") %}
                    {% set slatPosition = state_attr(entity_id,"current_tilt_position") %}
                    {% if value_json['properties']['shutterPosition'] is defined %}
                        {% set shutterPosition = value_json['properties']['shutterPosition'] %}
                    {% endif %}
                    {% if value_json['properties']['slatPosition'] is defined %}
                        {% set slatPosition = value_json['properties']['slatPosition'] %}
                    {% endif %}
                        
                    {
                      "position" : {{shutterPosition}}
                      "tilt_value" : {{slatPosition}}
                    }
                """;

                payload.Remove("stat_t");
            }      
        }
    }
}
