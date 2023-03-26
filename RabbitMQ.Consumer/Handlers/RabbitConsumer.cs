using System.Text;
using System.Threading.Channels;
using Newtonsoft.Json;
using RabbitMQ.Adapter;
using RabbitMQ.Domain.Jobs;
using RabbitMQ.Domain.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQ.Consumer.Handlers
{
    public static class RabbitConsumer
    {
        private static IModel? _channel = null;
        private static IServiceProvider? _services = null;

        public static void Configure(IServiceProvider services)
        {
            _services = services;
        }

        public static void CreateConsumer(string host, int port, string queue)
        {
            var client = new RabbitMqClient(connectionString: "localhost", port: 5672);

            if (_channel == null)
            {
                _channel = client.GetChannel(client.GetConnection());

                if (_channel.ConsumerCount(queue) == 0)
                {
                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += Consumer_Received;

                    _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
                }
            }
        }

        private static void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            if (_services == null)
            {
                throw new ArgumentNullException("Configuration is null");
            }

            var enqueue = _services.GetRequiredService<IEnqueueProcessMetric>();

            if (_channel != null)
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                switch (e.RoutingKey)
                {
                    case "metrics":
                        var metric = JsonConvert.DeserializeObject<Metric>(message);

                        if (metric != null)
                        {
                            enqueue.Invoke(metric);

                            Console.WriteLine($"Message received: {JsonConvert.SerializeObject(metric)} on queue {e.RoutingKey}");
                        }
                        break;
                    default:
                        break;
                }

                _channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
        }
    }
}

