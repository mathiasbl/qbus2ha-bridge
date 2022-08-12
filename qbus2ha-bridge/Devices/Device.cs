using System.Text.Json.Nodes;
using static qbus2ha.Services.DiscoveryService;

namespace qbus2ha.Devices
{
    abstract class Device : IDevice
    {
        public string Id { get; }

        public abstract string ComponentName { get; }

        public Device(string id)
        {
            Id = id;
        }

        public JsonObject GetConfigurationPayload(QbusFunctionBlock qbusFunctionBlock)
        {
            var baseTopic = $"~/{Id}";

            var uniqueId = qbusFunctionBlock.name.Replace("-", "_");
            var name = $"Qbus_{uniqueId.Replace(@"\", "_").Replace(".", "_")}";

            var payload = new JsonObject
            {
                ["name"] = name,
                ["cmd_t"] = $"{baseTopic}/setState",
                ["stat_t"] = $"{baseTopic}/state",
                ["uniq_id"] = uniqueId
            };

            return payload;
        }

        public abstract void AddDeviceProperties(QbusFunctionBlock qbusFunctionBlock, JsonObject payload);

    }
}
