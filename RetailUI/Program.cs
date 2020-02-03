using System;
using System.Threading.Tasks;
using Messages.Commands;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Routing.TypeBased;
using Serilog;
using Serilog.Core;

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

            var activator = new BuiltinHandlerActivator();
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            Configure.With(activator)
                .Transport(t => t.UseRabbitMqAsOneWayClient("amqp://guest:guest@localhost/RebusRetailDemo"))
                .Routing(r => r.TypeBased().Map<PlaceOrderCommand>("sales"))
                .Logging(l => l.Serilog(Log.Logger))
                .Start();

            //  Configure.With(activator)
            //      .Logging(l => l.ColoredConsole());

            await RunLoop(activator.Bus)
                .ConfigureAwait(false);
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