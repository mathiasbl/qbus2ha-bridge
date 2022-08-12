using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using qbus2ha.Options;

namespace qbus2ha.Services
{
    internal class MqttClientHostedService : IHostedService
    {
        private readonly ILogger<MqttClientHostedService> logger;
        private readonly IManagedMqttClient managedMqttClient;
        private readonly IOptions<MqttOptions> mqttOptions;

        public MqttClientHostedService(
            ILogger<MqttClientHostedService> logger,
            IManagedMqttClient managedMqttClient,
            IOptions<MqttOptions> mqttOptions )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.managedMqttClient = managedMqttClient ?? throw new ArgumentNullException(nameof(managedMqttClient));
            this.mqttOptions = mqttOptions ?? throw new ArgumentNullException(nameof(mqttOptions));
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var options = mqttOptions.Value;

            // Setup and start a managed MQTT client.
            var ManagedMqttClientOptions =  new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("qbus2ha.bridge")
                    .WithTcpServer(options.Host, options.Port)
                    .WithCredentials(options.Username, options.Password)
                    .WithTls().Build())
                .Build();

            await managedMqttClient.StartAsync(ManagedMqttClientOptions);
        }

        public Task StopAsync(CancellationToken cancellationToken) => managedMqttClient.StopAsync();
    }
}
