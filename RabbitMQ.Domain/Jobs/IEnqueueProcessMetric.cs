using RabbitMQ.Domain.Models;

namespace RabbitMQ.Domain.Jobs
{
    public interface IEnqueueProcessMetric
    {
        void Invoke(Metric value);
    }
}

