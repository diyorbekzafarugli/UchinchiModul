using AutoMapper;
using PostsSocialMedia.Api.Dtos.CommentDto;
using PostsSocialMedia.Api.Entities.Comment;

namespace PostsSocialMedia.Api.Mappings;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentGetDto>();
    }
}
