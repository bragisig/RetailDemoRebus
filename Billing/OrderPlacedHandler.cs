using System.Threading.Tasks;
using Messages.Events;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Billing
{
    public class OrderPlacedHandler :
        IHandleMessages<OrderPlacedEvent>
    {
        private readonly IBus bus;
        private readonly ILogger logger;

        public OrderPlacedHandler(IBus bus, ILogger<OrderPlacedHandler> logger)
        {
            this.bus = bus;
            this.logger = logger;
        }
        
        public Task Handle(OrderPlacedEvent message)
        {
            logger.LogInformation($"Received OrderPlacedEvent, OrderId = {message.OrderId}");

            var orderBilledEvent = new OrderBilledEvent()
            {
                OrderId = message.OrderId
            };
            
            bus.Publish(orderBilledEvent);
            
            return Task.CompletedTask;
        }
    }
}