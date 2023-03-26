using Hangfire;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Application.Services;
using RabbitMQ.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile($"appsettings.json", optional: true);

builder.Services.AddRazorPages();

// Add Services
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetValue<string>("HangfireConnectionString")));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IProcessMetric, ProcessMetric>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Use hangfire dashboard
app.UseHangfireDashboard("/hangfire");

app.Run();