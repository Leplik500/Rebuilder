using System.Net;
using AutoMapper;
using MediatR;
using Pepegov.MicroserviceFramework.ApiResults;
using Pepegov.UnitOfWork;
using Pepegov.UnitOfWork.EntityFramework;
using PublicWorkout.Application.Query;
using PublicWorkout.Domain.Aggregate;
using PublicWorkout.Domain.Entity;
using PublicWorkout.Domain.Event;

namespace PublicWorkout.Application.Handler;

public class OrderCreateCommandHandler
    : IRequestHandler<OrderCreateCommand, ApiResult>
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public OrderCreateCommandHandler(
        IUnitOfWorkManager unitOfWorkManager,
        IMapper mapper,
        IPublisher publisher
    )
    {
        this._unitOfWorkManager = unitOfWorkManager;
        this._mapper = mapper;
        this._publisher = publisher;
    }

    public async Task<ApiResult> Handle(
        OrderCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var unitOfWorkInstance =
            this._unitOfWorkManager.GetInstance<IUnitOfWorkEntityFrameworkInstance>();
        var orderRepository = unitOfWorkInstance.GetRepository<Order>();

        var order = new Order(request.CreationDto.Id);
        var products = this._mapper.Map<List<Product>>(request.CreationDto.Products);
        products.ForEach(item => order.AddProduct(item));

        await orderRepository.InsertAsync(order, cancellationToken);
        await unitOfWorkInstance.SaveChangesAsync(cancellationToken);

        if (!unitOfWorkInstance.LastSaveChangesResult.IsOk)
        {
            var exception = unitOfWorkInstance.LastSaveChangesResult.Exception!;
            var errorMessage =
                $"Unable to save changes to database | exception: {exception}";
            return new ApiResult(
                HttpStatusCode.InternalServerError,
                new Exception(errorMessage, exception)
            );
        }

        await this._publisher.Publish(
            new OrderCreatedEvent(order.Id, order.CreationDate),
            cancellationToken: cancellationToken
        );
        return new ApiResult(HttpStatusCode.OK);
    }
}
