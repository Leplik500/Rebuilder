using Microsoft.EntityFrameworkCore;
using Pepegov.UnitOfWork;
using Pepegov.UnitOfWork.EntityFramework;
using PublicWorkout.Application.Services.Interfaces;
using PublicWorkout.Domain.Aggregate;
using PublicWorkout.Infrastructure.Exceptions;

namespace PublicWorkout.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public OrderService(IUnitOfWorkManager unitOfWorkManager)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<Order> GetOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var unitOfWorkInstance =
            _unitOfWorkManager.GetInstance<IUnitOfWorkEntityFrameworkInstance>();
        unitOfWorkInstance.SetAutoDetectChanges(false);

        var orderRepository = unitOfWorkInstance.GetRepository<Order>();
        var order = (
            await orderRepository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == id,
                include: include => include.Include(i => i.Items).Include(i => i.Taxes),
                cancellationToken: cancellationToken
            )
        );

        if (order is null)
        {
            throw new NotFoundException($"Order by id {id} was not found");
        }

        return order;
    }
}
