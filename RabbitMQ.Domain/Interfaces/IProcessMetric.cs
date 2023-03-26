using RabbitMQ.Domain.Models;

namespace RabbitMQ.Domain.Interfaces
{
    public interface IProcessMetric
    {
        Task InvokeAsync(Metric value);
    }
}

