using MediatR;
using Pepegov.MicroserviceFramework.ApiResults;

namespace User.Application.Query;

public record OrderGetTotalPriceRequest(Guid Id) : IRequest<ApiResult<decimal>>;
