using System;
using Messages.Events;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Serilog;

namespace Shipping
{
    class Program
    {
        static void Main(string[] args)
        {
            var activator = new BuiltinHandlerActivator();
            
            activator.Register(() => new ShippingSaga());

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            Configure.With(activator)
                .Transport(t => t.UseRabbitMq("amqp://guest:guest@localhost/RebusRetailDemo","shipping"))
                .Logging(l => l.Serilog(Log.Logger))
                .Sagas(s => s.StoreInMemory())
                .Start();
            
            activator.Bus.Subscribe<OrderPlacedEvent>().Wait();
            activator.Bus.Subscribe<OrderBilledEvent>().Wait();

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}