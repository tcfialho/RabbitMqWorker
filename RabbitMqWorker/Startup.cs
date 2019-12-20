using BuildingBlock.MessageQueue.Rabbittmq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RabbitMqWorker.Entities;
using RabbitMqWorker.Tasks;

namespace RabbitMqWorker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<TestMessageTask>();

            services.AddMessageQueueRabbitMq(setup =>
            {
                var options = Configuration.GetSection("RabbitMQ").Get<MessageQueueOptions>();

                setup.Host = options.Host;
                setup.Port = options.Port;
                setup.ExchangeName = options.ExchangeName;
                setup.QueueName = options.QueueName;
                setup.Username = options.Username;
                setup.Password = options.Password;
                setup.CreateIfNotExists = true;
            });

            services.AddHealthChecks()
                    .AddRabbitMqCheck<TestMessage>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/hc");
        }
    }
}
