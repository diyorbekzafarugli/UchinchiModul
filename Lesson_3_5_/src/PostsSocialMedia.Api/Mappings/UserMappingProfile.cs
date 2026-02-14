using AutoMapper;
using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities.User;

namespace PostsSocialMedia.Api.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserGetDto>();

        CreateMap<UserUpdateDto, User>();
    }
}
