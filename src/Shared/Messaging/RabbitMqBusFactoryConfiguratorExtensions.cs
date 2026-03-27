using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Vimachem.Messaging;

public static class RabbitMqBusFactoryConfiguratorExtensions
{
    public static void ConfigureVimachemRabbitMqHost(
        this IRabbitMqBusFactoryConfigurator cfg,
        IConfiguration configuration)
    {
        var rabbitConn = configuration.GetConnectionString("rabbitmq");
        if (!string.IsNullOrWhiteSpace(rabbitConn)
            && Uri.TryCreate(rabbitConn, UriKind.Absolute, out var rabbitUri))
        {
            cfg.Host(rabbitUri);
        }
        else
        {
            cfg.Host(configuration["RabbitMq:Host"] ?? "localhost", h =>
            {
                h.Username(configuration["RabbitMq:Username"] ?? "guest");
                h.Password(configuration["RabbitMq:Password"] ?? "guest");
            });
        }
    }
}
