using System;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.ServiceProvider;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Sales
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
            services.AutoRegisterHandlersFromAssemblyOf<Sales.Program>();
            
            services.AddRebus(configure => configure
                .Logging(l => l.Serilog(Log.Logger))
                .Transport(t => t.UseRabbitMq("amqp://guest:guest@localhost/RetailDemoRebus","sales")));
            
            using (var provider = services.BuildServiceProvider())
            {
                provider.UseRebus();
                
                Console.ReadLine();
            }
        }
    }
}