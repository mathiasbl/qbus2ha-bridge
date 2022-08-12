using qbus2ha.Services;
using System.Text.Json.Nodes;

namespace qbus2ha.Devices
{
    internal interface IDevice
    {
        string Id { get;}
        string ComponentName { get; }

        JsonObject GetConfigurationPayload(DiscoveryService.QbusFunctionBlock qbusFunctionBlock);
    }
}