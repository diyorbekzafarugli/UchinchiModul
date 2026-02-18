using AutoMapper;
using PostsSocialMedia.Api.Dtos.PostDto;
using PostsSocialMedia.Api.Entities.Post;

namespace PostsSocialMedia.Api.Mappings;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<Post, PostGetDto>();

        CreateMap<PostUpdateDto, Post>();
    }
}
