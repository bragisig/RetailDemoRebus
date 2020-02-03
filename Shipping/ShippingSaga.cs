using System.Threading.Tasks;
using Messages.Events;
using Rebus.Handlers;
using Rebus.Sagas;
using Serilog;

namespace Shipping
{
    /// <summary>
    /// Rebus docs: https://github.com/rebus-org/Rebus/wiki/Coordinating-stuff-that-happens-over-time
    /// </summary>
    public class ShippingSaga : Saga<CollectShippingData>,
        IAmInitiatedBy<OrderPlacedEvent>,
        IHandleMessages<OrderBilledEvent>
    {
        public async Task Handle(OrderPlacedEvent message)
        {
            Log.Logger.Information($"Shipping received OrderPlacedEvent, OrderId = {message.OrderId}");
            Data.OrderId = message.OrderId;
            Data.OrderPlaced = true;
            await PossiblyPerformCompleteAction();
        }

        public async Task Handle(OrderBilledEvent message)
        {
            Log.Logger.Information($"Shipping received OrderBilledEvent, OrderId = {message.OrderId}");
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
                Log.Logger.Information($"Order ready for shipping, OrderId = {Data.OrderId}");
                MarkAsComplete();
            }
            
            return Task.CompletedTask;
        }
    }
}