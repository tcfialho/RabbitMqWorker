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
        private readonly AsyncEventingBasicConsumer _consumer;

        public Worker(IModel channel,
                      ILogger<Worker> logger,
                      RabbitMQConfig rabbitMQConfig)
        {
            _channel = channel;
            _logger = logger;
            _rabbitMQConfig = rabbitMQConfig;
            _consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(_rabbitMQConfig.QueueName, false, _consumer);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _consumer.Received += async (sender, args) =>
            {
                _logger.LogInformation("Received {@content} as consumer {@consumer}", Encoding.UTF8.GetString(args.Body), args.ConsumerTag);
                _channel.BasicAck(args.DeliveryTag, false);
                await Task.CompletedTask;
            };

            await Task.CompletedTask;
        }
    }
}