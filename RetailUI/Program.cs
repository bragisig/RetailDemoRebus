using System;
using System.Threading.Tasks;
using Messages.Commands;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace RetailUI
{
 class Program
    {
        
        static async Task Main(string[] args)
        {
            try
            {
                AsyncMain().GetAwaiter().GetResult();

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }
        
        static async Task AsyncMain()
        {
            Console.Title = "RebusRetailDemoUI";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .MinimumLevel.Debug()
                .CreateLogger();
            
            var services = new ServiceCollection();
            services.AddRebus(configure => configure
                .Logging(l => l.Serilog(Log.Logger))
                .Transport(t => t.UseRabbitMqAsOneWayClient("amqp://guest:guest@localhost/RetailDemoRebus"))
                .Routing(r => r.TypeBased().Map<PlaceOrderCommand>("sales")));
            
            using (var provider = services.BuildServiceProvider())
            {
                provider.UseRebus();
                
                 await RunLoop(provider.GetService<IBus>())
                     .ConfigureAwait(false);
            }
        }
        
        static async Task RunLoop(IBus bus)
        {
            while (true)
            {
                Console.WriteLine("Press 'P' to place an order, or 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.P:
                        // Instantiate the command
                        var command = new PlaceOrderCommand
                        {
                            OrderId = Guid.NewGuid()
                        };

                        // Send the command to the local endpoint
                        Console.WriteLine($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                        await bus.Send(command).ConfigureAwait(false);

                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        Console.WriteLine("Unknown input. Please try again.");
                        break;
                }
            }
        }
    }
}