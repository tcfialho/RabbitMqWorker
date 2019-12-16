using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;

namespace RabbitMqWorker
{
    public class RabbitMQConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    var rabbitConfig = hostContext.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();

                    services.AddSingleton<RabbitMQConfig>(rabbitConfig);

                    services.AddSingleton<IConnection>(serviceProvider =>
                    {
                        var config = serviceProvider.GetRequiredService<RabbitMQConfig>();
                        var factory = new ConnectionFactory()
                        {
                            HostName = config.Host,
                            Port = config.Port,
                            UserName = config.Username,
                            Password = config.Password,
                            DispatchConsumersAsync = true
                        };
                        return factory.CreateConnection();
                    });

                    services.AddTransient<IModel>(serviceProvider =>
                    {
                        var config = serviceProvider.GetRequiredService<RabbitMQConfig>();
                        var connection = serviceProvider.GetRequiredService<IConnection>();
                        var channel = connection.CreateModel();

                        channel.ExchangeDeclare(config.ExchangeName, ExchangeType.Fanout, true);
                        channel.QueueDeclare(config.QueueName, true, false, false, null);
                        channel.QueueBind(config.QueueName, config.ExchangeName, "", null);
                        channel.BasicQos(0, 1, false);

                        return channel;
                    });
                });
    }
}
