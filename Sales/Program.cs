using System;
using Messages.Events;
using Rebus.Activation;
using Rebus.Config;
using Serilog;

namespace Sales
{
    class Program
    {
        static void Main(string[] args)
        {
            // we have the container in a variable, but you would probably stash it in a static field somewhere
            var activator = new BuiltinHandlerActivator();
            
            activator.Register(() => new PlaceOrderHandler(activator.Bus));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            Configure.With(activator)
                .Transport(t => t.UseRabbitMq("amqp://guest:guest@localhost/RebusRetailDemo","sales"))
                .Logging(l => l.Serilog(Log.Logger))
                .Start();

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
           
        }
    }
}