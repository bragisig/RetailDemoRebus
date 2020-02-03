using System;

namespace Messages.Events
{
    public class OrderPlacedEvent
    {
        public Guid OrderId { get; set; }
    }
}