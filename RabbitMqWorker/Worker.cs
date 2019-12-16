using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqWorker
{
    public class Worker : BackgroundService
    {
        private readonly IModel _channel;
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQConfig _rabbitMQConfig;

        public Worker(IModel channel,
                      ILogger<Worker> logger,
                      RabbitMQConfig rabbitMQConfig)
        {
            _channel = channel;
            _logger = logger;
            _rabbitMQConfig = rabbitMQConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += Consumer_Received;

            _channel.BasicConsume(_rabbitMQConfig.QueueName, false, consumer);

            await Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            _logger.LogInformation("Received {@content} as consumer {@consumer}", Encoding.UTF8.GetString(args.Body), args.ConsumerTag);
            _channel.BasicAck(args.DeliveryTag, false);
        }
    }
}