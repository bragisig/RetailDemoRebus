using System;
using System.Threading;
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
        Random rnd = new Random();
        
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

            //Simulate processing time
            Thread.Sleep(rnd.Next(0, 2000));
            
            //Make the charging of the card fail randomly to test the retry mechanism
            if (rnd.Next(0, 8) == 0)
            {
                throw new Exception("Credit card charging failed!");
            }
            
            var orderBilledEvent = new OrderBilledEvent()
            {
                OrderId = message.OrderId
            };
            
            bus.Publish(orderBilledEvent);
            
            return Task.CompletedTask;
        }
    }
}