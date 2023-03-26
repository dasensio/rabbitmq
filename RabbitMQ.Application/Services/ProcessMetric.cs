using RabbitMQ.Domain.Interfaces;
using RabbitMQ.Domain.Models;

namespace RabbitMQ.Application.Services
{
    public class ProcessMetric : IProcessMetric
    {
        public ProcessMetric()
        {
        }

        public async Task InvokeAsync(Metric value)
        {
            // Do some tasks
            await Task.Delay(1000);
        }
    }
}

