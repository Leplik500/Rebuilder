using PublicWorkout.Domain.Aggregate;

namespace PublicWorkout.Domain.Service;

public class OrderDomainService
{
    public bool CanOrderBeShipped(Order order)
    {
        return order.CreationDate <= DateTime.Now;
    }
}
