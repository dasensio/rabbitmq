using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Application.Services;
using RabbitMQ.Consumer.Handlers;
using RabbitMQ.Consumer.Jobs;
using RabbitMQ.Domain.Interfaces;
using RabbitMQ.Domain.Jobs;

var builder = Host.CreateDefaultBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

builder.ConfigureServices(services => {
    services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetValue<string>("HangfireConnectionString")));
    services.AddScoped<IEnqueueProcessMetric, EnqueueProcessMetric>();
    services.AddScoped<IProcessMetric, ProcessMetric>();
});

var host = builder.Build();

// Configure consumer
RabbitConsumer.Configure(host.Services);
RabbitConsumer.CreateConsumer(host: "localhost", port: 5672, queue: "metrics");

Console.ReadLine();