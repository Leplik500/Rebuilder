using PublicWorkout.Infrastructure;

namespace PublicWorkout.Domain.Event;

public class OrderCreatedEvent(Guid orderId, DateTime creationDate) : IEvent
{
    public Guid OrderId { get; } = orderId;
    public DateTime CreationDate { get; } = creationDate;
}
