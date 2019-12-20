using BuildingBlock.MessageQueue;

using Microsoft.Extensions.Hosting;

using RabbitMqWorker.Entities;

using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqWorker.Tasks
{
    public class TestMessageTask : BackgroundService
    {
        private readonly IMessageQueue _queue;
        public TestMessageTask(IMessageQueue queue)
        {
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await foreach (var message in _queue.Receive<TestMessage>())
                {
                    await Task.Delay(1000);
                    await _queue.Dequeue(message);
                }

                await Task.Delay(1000);
            }
        }
    }
}