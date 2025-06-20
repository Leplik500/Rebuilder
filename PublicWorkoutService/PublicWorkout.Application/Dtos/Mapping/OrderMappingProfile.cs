using AutoMapper;
using PublicWorkout.Domain.Entity;

namespace PublicWorkout.Application.Dtos.Mapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        this.CreateMap<ProductDto, Product>().ReverseMap();
    }
}
