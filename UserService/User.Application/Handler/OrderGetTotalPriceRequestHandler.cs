// using System.Net;
// using MediatR;
// using Pepegov.MicroserviceFramework.ApiResults;
// using UserEntity.Application.Query;
// using UserEntity.Application.Services.Interfaces;
// using UserEntity.Infrastructure.Exceptions;
//
// namespace UserEntity.Application.Handler;
//
// public class OrderGetTotalPriceRequestHandler
//     : IRequestHandler<OrderGetTotalPriceRequest, ApiResult<decimal>>
// {
//     private readonly IOrderService _orderService;
//
//     public OrderGetTotalPriceRequestHandler(IOrderService orderService)
//     {
//         _orderService = orderService;
//     }
//
//     public async Task<ApiResult<decimal>> Handle(
//         OrderGetTotalPriceRequest request,
//         CancellationToken cancellationToken
//     )
//     {
//         try
//         {
//             var order = await _orderService.GetOrderByIdAsync(
//                 request.Id,
//                 cancellationToken
//             );
//             return new ApiResult<decimal>(order.GetTotalPrice(), HttpStatusCode.OK);
//         }
//         catch (NotFoundException exception)
//         {
//             return new ApiResult<decimal>(HttpStatusCode.NotFound, exception);
//         }
//     }
// }
