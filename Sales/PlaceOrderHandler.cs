using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Sales
{
    public class PlaceOrderHandler :
        IHandleMessages<PlaceOrderCommand>
    {
        private readonly IBus bus;
        private readonly ILogger<PlaceOrderHandler> logger;

        public PlaceOrderHandler(IBus bus, ILogger<PlaceOrderHandler> logger)
        {
            this.bus = bus;
            this.logger = logger;
        }

        public Task Handle(PlaceOrderCommand message)
        {
            logger.LogInformation($"Received PlaceOrderCommand, OrderId = {message.OrderId}");

            var orderPlacedEvent = new OrderPlacedEvent
            {
                OrderId = message.OrderId
            };
            
            bus.Publish(orderPlacedEvent);
            
            return Task.CompletedTask;
        }
    }
}