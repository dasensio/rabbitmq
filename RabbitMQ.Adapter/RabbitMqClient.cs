using System.Text;
using System.Threading.Channels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Adapter
{
    public class RabbitMqClient
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqClient(string connectionString, int port)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException();
            }

            _factory = new ConnectionFactory()
            {
                HostName = connectionString,
                Port = port
            };
        }

        public IConnection GetConnection()
        {
            return _factory.CreateConnection();
        }

        public IModel GetChannel(IConnection connection)
        {
            return connection.CreateModel();
        }

        public void Publish<T>(string queue, T message)
        {
            using (var connection = GetConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                    channel.BasicPublish(exchange: string.Empty, routingKey: queue, basicProperties: null, body: body);
                }
            }
        }

        public IEnumerable<T> Get<T>(string queue)
        {
            List<T> result = new();

            using var connection = GetConnection();
            using var channel = GetChannel(connection);

            while (TryGetResult(queue, channel, out BasicGetResult getResult))
            {
                var model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(getResult.Body.ToArray()));

                if (model != null)
                {
                    result.Add(model);

                    channel.BasicAck(deliveryTag: getResult.DeliveryTag, multiple: false);
                }
            }

            return result;
        }

        private static bool TryGetResult(string queue, IModel channel, out BasicGetResult result)
        {
            result = channel.BasicGet(queue, false);

            return result != null;
        }
    }
}

