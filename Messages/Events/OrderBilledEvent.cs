using System;

namespace Messages.Events
{
    public class OrderBilledEvent
    {
        public Guid OrderId { get; set; }
    }
}