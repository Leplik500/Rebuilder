using User.Infrastructure;

namespace User.Domain.Event;

public class OrderCreatedEvent(Guid orderId, DateTime creationDate) : IEvent
{
    public Guid OrderId { get; } = orderId;
    public DateTime CreationDate { get; } = creationDate;
}
