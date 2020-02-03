using System;
using Messages.Events;
using Rebus.Activation;
using Rebus.Config;
using Serilog;

namespace Billing
{
    class Program
    {
        static void Main(string[] args)
        {
            var activator = new BuiltinHandlerActivator();
            activator.Register(() => new OrderPlacedHandler(activator.Bus));
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            Configure.With(activator)
                .Transport(t => t.UseRabbitMq("amqp://guest:guest@localhost/RebusRetailDemo", "billing"))
                .Logging(l => l.Serilog(Log.Logger))
                .Start();

            activator.Bus.Subscribe<OrderPlacedEvent>().Wait();
            
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}