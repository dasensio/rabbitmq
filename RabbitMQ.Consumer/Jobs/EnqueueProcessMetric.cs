using Hangfire;
using RabbitMQ.Domain.Interfaces;
using RabbitMQ.Domain.Jobs;
using RabbitMQ.Domain.Models;

namespace RabbitMQ.Consumer.Jobs
{
    public class EnqueueProcessMetric : IEnqueueProcessMetric
    {
        private readonly IProcessMetric _service;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public EnqueueProcessMetric(IProcessMetric service, IBackgroundJobClient backgroundJobClient)
        {
            _service = service;
            _backgroundJobClient = backgroundJobClient;
        }

        public void Invoke(Metric value)
        {
            _backgroundJobClient.Enqueue(() => _service.InvokeAsync(value));
        }
    }
}

