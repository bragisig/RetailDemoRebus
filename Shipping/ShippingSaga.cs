using System;
using System.Threading;
using System.Threading.Tasks;
using Messages.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Rebus.Sagas;

namespace Shipping
{
    /// <summary>
    /// Rebus docs: https://github.com/rebus-org/Rebus/wiki/Coordinating-stuff-that-happens-over-time
    /// </summary>
    public class ShippingSaga : Saga<CollectShippingData>,
        IAmInitiatedBy<OrderPlacedEvent>,
        IHandleMessages<OrderBilledEvent>
    {
        Random rnd = new Random();
        private readonly ILogger<ShippingSaga> logger;

        public ShippingSaga(ILogger<ShippingSaga> logger)
        {
            this.logger = logger;
        }

        public async Task Handle(OrderPlacedEvent message)
        {
            logger.LogInformation($"Shipping received OrderPlacedEvent, OrderId = {message.OrderId}");

            //Simulate processing time
            Thread.Sleep(rnd.Next(0, 2000));
            
            Data.OrderId = message.OrderId;
            Data.OrderPlaced = true;
            await Task.CompletedTask;
        }

        public async Task Handle(OrderBilledEvent message)
        {
            logger.LogInformation($"Shipping received OrderBilledEvent, OrderId = {message.OrderId}");
            
            //Simulate processing time
            Thread.Sleep(rnd.Next(0, 2000));
            
            Data.OrderId = message.OrderId;
            Data.OrderBilled = true;
            await PossiblyPerformCompleteAction();
        }

        protected override void CorrelateMessages(ICorrelationConfig<CollectShippingData> config)
        {
            config.Correlate<OrderPlacedEvent>(e => e.OrderId, data => data.OrderId);
            config.Correlate<OrderBilledEvent>(e => e.OrderId, data => data.OrderId);
        }
        
        private Task PossiblyPerformCompleteAction()
        {
            if (Data.OrderPlaced && Data.OrderBilled)
            {
                logger.LogInformation($"Order ready for shipping, OrderId = {Data.OrderId}");
                MarkAsComplete();
            }
            
            return Task.CompletedTask;
        }
    }
}