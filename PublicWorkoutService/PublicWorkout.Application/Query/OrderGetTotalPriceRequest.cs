using MediatR;
using Pepegov.MicroserviceFramework.ApiResults;

namespace PublicWorkout.Application.Query;

public record OrderGetTotalPriceRequest(Guid Id) : IRequest<ApiResult<decimal>>;
