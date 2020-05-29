using System;
using Messages.Events;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.ServiceProvider;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Billing
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .MinimumLevel.Debug()
                .CreateLogger();
            
            var services = new ServiceCollection();
            services.AddLogging(l => l.AddSerilog(Log.Logger));
            services.AutoRegisterHandlersFromAssemblyOf<Billing.Program>();

            services.AddRebus(configure => configure
                .Logging(l => l.Serilog(Log.Logger))
                .Transport(t => t.UseRabbitMq("amqp://guest:guest@localhost/RetailDemoRebus", "billing"))
                .Options(b => b.SimpleRetryStrategy(maxDeliveryAttempts: 3)));
            
            using (var provider = services.BuildServiceProvider())
            {
                provider.UseRebus(async bus => await bus.Subscribe<OrderPlacedEvent>());
                
                Console.ReadLine();
            }
        }
    }
}