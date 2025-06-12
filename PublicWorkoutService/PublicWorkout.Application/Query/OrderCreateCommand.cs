using MediatR;
using Pepegov.MicroserviceFramework.ApiResults;
using PublicWorkout.Application.Dtos;

namespace PublicWorkout.Application.Query;

public record OrderCreateCommand(OrderCreationDto CreationDto) : IRequest<ApiResult>;
