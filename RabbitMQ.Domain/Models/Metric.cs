namespace RabbitMQ.Domain.Models
{
    public class Metric
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}

