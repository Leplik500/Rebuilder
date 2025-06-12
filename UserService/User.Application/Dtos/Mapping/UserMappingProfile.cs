using AutoMapper;
using User.Domain.Entity;

namespace User.Application.Dtos.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        this.CreateMap<RegisterUserDto, UserEntity>().ReverseMap();
        this.CreateMap<UserDto, UserEntity>().ReverseMap();
        this.CreateMap<UpdateUserSettingsDto, UserSettings>().ReverseMap();
        this.CreateMap<UserSettings, UserSettingsDto>().ReverseMap();
        this.CreateMap<UpdateUserProfileDto, UserProfile>().ReverseMap();
        this.CreateMap<UserProfileDto, UserProfile>().ReverseMap();
    }
}
