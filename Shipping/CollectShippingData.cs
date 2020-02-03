using System;
using Rebus.Sagas;

namespace Shipping
{
    public class CollectShippingData : ISagaData
    {
        public Guid Id { get; set; }
        public int Revision { get; set; }
        
        public Guid OrderId { get; set; }
        public bool OrderPlaced { get; set; }
        public bool OrderBilled { get; set; }
    }
}