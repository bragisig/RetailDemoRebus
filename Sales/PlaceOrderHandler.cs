using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Pipeline;
using Serilog;

namespace Sales
{
    public class PlaceOrderHandler :
        IHandleMessages<PlaceOrderCommand>
    {
        private readonly IBus bus;

        public PlaceOrderHandler(IBus bus)
        {
            this.bus = bus;
        }

        public Task Handle(PlaceOrderCommand message)
        {
            Log.Logger.Information($"Received PlaceOrderCommand, OrderId = {message.OrderId}");

            var orderPlacedEvent = new OrderPlacedEvent
            {
                OrderId = message.OrderId
            };
            
            bus.Publish(orderPlacedEvent);
            
            return Task.CompletedTask;
        }
    }
}