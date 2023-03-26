using Newtonsoft.Json;
using RabbitMQ.Adapter;
using RabbitMQ.Domain.Models;

var client = new RabbitMqClient(connectionString: "localhost", port: 5672);
var queue = "metrics";

while (true)
{
    Task.Delay(1000).GetAwaiter().GetResult();

    var metric = new Metric()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "CustomMetric",
        Value = new Random().Next()
    };

    client.Publish(queue, metric);

    Console.WriteLine($"Message sent: {JsonConvert.SerializeObject(metric)} on queue {queue}");
}