using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using qbus2ha.Options;
using qbus2ha.Services;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddOptions<MqttOptions>("Mqtt");
        services.AddOptions<QbusOptions>("Qbus");
        services.AddOptions<HomeAssistantOptions>("HomeAssistant");

        services.AddSingleton<MqttFactory>();
        services.AddSingleton(sp => sp.GetRequiredService<MqttFactory>().CreateManagedMqttClient());

        services.AddHostedService<MqttClientHostedService>();
        services.AddHostedService<DiscoveryService>();
    })
    .Build()
    .RunAsync();