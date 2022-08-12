using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using qbus2ha.Devices;
using qbus2ha.Options;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace qbus2ha.Services
{
    internal class DiscoveryService : BackgroundService
    {
        private const string GetConfigSuffix = $"getConfig";
        private const string ConfigSuffix = $"config";

        // todo from config
        private static readonly TimeSpan Delay = TimeSpan.FromMinutes(30);

        private readonly IManagedMqttClient _managedMqttClient;
        private readonly IOptions<QbusOptions> _qbusOptions;
        private readonly IOptions<HomeAssistantOptions> _homeAssistantOptions;

        public DiscoveryService(IManagedMqttClient managedMqttClient, 
            IOptions<QbusOptions> qbusOptions,
            IOptions<HomeAssistantOptions> homeAssistantOptions)
        {
            _managedMqttClient = managedMqttClient;
            _qbusOptions = qbusOptions;
            _homeAssistantOptions = homeAssistantOptions;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _managedMqttClient.ApplicationMessageProcessedAsync += ManagedMqttClient_ApplicationMessageProcessedAsync;

            // subscribe to the config topic from qbus
            await _managedMqttClient.SubscribeAsync($"{_qbusOptions.Value.BaseTopic}/{ConfigSuffix}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic($"{_qbusOptions.Value.BaseTopic}/{GetConfigSuffix}")
                    .WithPayload(string.Empty)
                    .Build();

                // send the getconfig message
                await _managedMqttClient.EnqueueAsync(message);


                await Task.Delay(Delay, stoppingToken);
            }
        }

        public async Task ManagedMqttClient_ApplicationMessageProcessedAsync(ApplicationMessageProcessedEventArgs arg)
        {
            var configurationMessage = JsonSerializer.Deserialize<QbusConfigurationMessage>(arg.ApplicationMessage.ApplicationMessage.Payload);
            if (configurationMessage == null)
                return;

            foreach (var functionBlock in configurationMessage.devices.SelectMany(d =>  d.functionBlocks))
            {
                var controllerId = string.Empty; // TODO
                IDevice? device = default;

                switch (functionBlock.type)
                {
                    case "onoff":
                        device = new Switch(functionBlock.id); 
                        break;
                    case "analog":
                        device = new Light(functionBlock.id);
                        break;
                    case "thermo":
                        device = new Climate(functionBlock.id);
                        break;
                    case "scene":
                        device = new Scene(functionBlock.id);
                        break;
                    case "humidity":
                        device = new HumiditySensor(functionBlock.id);
                        break;
                    case "ventilation":
                        device = new CO2Sensor(functionBlock.id);
                        break;
                    //case "gauge":
                    //    // sensor (energy?)
                    //    break;
                    case "shutter":
                        device = new Cover(functionBlock.id);
                        break;
                    default:
                        break;
                }

                if (device == null)
                    continue;

                var topic = $"{_homeAssistantOptions.Value.DiscoveryPrefix}/{device.ComponentName}/{device.Id}/{ConfigSuffix}";
                var payload = device.GetConfigurationPayload(functionBlock);

                payload["~"] = $"{_qbusOptions.Value.BaseTopic}/{controllerId}";

                var message = new MqttApplicationMessageBuilder()
                                    .WithTopic(topic)
                                    .WithRetainFlag(true)
                                    .WithPayload(payload.ToString())
                                    .Build();

                await _managedMqttClient.EnqueueAsync(message);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _managedMqttClient.ApplicationMessageProcessedAsync -= ManagedMqttClient_ApplicationMessageProcessedAsync;

            return base.StopAsync(cancellationToken);
        }

        private class QbusConfigurationMessage
        {
            public QbusController[] controllers;
            public QbusDevice[] devices;
            public JsonObject properties;
            public string id;
            public JsonObject state;
            public string type;
            public string version;
        }

        public class QbusDevice
        {
            public string serialNr;
            public string id;
            public QbusFunctionBlock[] functionBlocks;
        }

        public class QbusFunctionBlock
        {
            public string refId;
            public string type;
            public string id;
            public string name;
            public string location;
            public string properties;
        }

        public class QbusController
        {
            public string auth;
            public string configFile;
            public string ctdVersion;
            public string ip;
            public string lastConfig;
            public string mac;
            public string serialNr;
            public string version;
        }
    }
}
