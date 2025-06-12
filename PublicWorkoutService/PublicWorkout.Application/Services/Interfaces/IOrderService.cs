using PublicWorkout.Domain.Aggregate;

namespace PublicWorkout.Application.Services.Interfaces;

public interface IOrderService
{
    Task<Order> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
