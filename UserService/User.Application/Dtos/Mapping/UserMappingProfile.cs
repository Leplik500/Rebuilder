using AutoMapper;
using User.Domain.Entity;
using User.Domain.Enum;

namespace User.Application.Dtos.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        this.CreateMap<UserEntity, RegisterUserDto>();
        this.CreateMap<UserEntity, UserDto>();
        this.CreateMap<UserSettings, UpdateUserSettingsDto>();
        this.CreateMap<UserSettings, UserSettingsDto>();
        // .ForMember(
        //     dest => dest.Theme,
        //     opt => opt.MapFrom(src => src.Theme.ToString())
        // )
        // .ForMember(
        //     dest => dest.Language,
        //     opt => opt.MapFrom(src => src.Language.ToString())
        // );
        this.CreateMap<UserProfile, UpdateUserProfileDto>();
        this.CreateMap<UserProfile, UserProfileDto>();

        this.CreateMap<Theme, string>().ConvertUsing(src => src.ToString());
        this.CreateMap<Language, string>().ConvertUsing(src => src.ToString());
        this.CreateMap<Gender, string>().ConvertUsing(src => src.ToString());
        this.CreateMap<ActivityLevel, string>().ConvertUsing(src => src.ToString());
        this.CreateMap<FitnessGoal, string>().ConvertUsing(src => src.ToString());
    }
}
