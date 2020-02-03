using System.Threading.Tasks;
using Messages.Events;
using Rebus.Bus;
using Rebus.Handlers;
using Serilog;

namespace Billing
{
    public class OrderPlacedHandler :
        IHandleMessages<OrderPlacedEvent>
    {
        private readonly IBus bus;

        public OrderPlacedHandler(IBus bus)
        {
            this.bus = bus;
        }
        
        public Task Handle(OrderPlacedEvent message)
        {
            Log.Logger.Information($"Received OrderPlacedEvent, OrderId = {message.OrderId}");

            var orderBilledEvent = new OrderBilledEvent()
            {
                OrderId = message.OrderId
            };
            
            bus.Publish(orderBilledEvent);
            
            return Task.CompletedTask;
        }
    }
}