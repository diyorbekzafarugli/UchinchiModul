using AutoMapper;
using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Entities.Reaction;

namespace PostsSocialMedia.Api.Mappings;

public class ReactionMappingProfile : Profile
{
    public ReactionMappingProfile()
    {
        CreateMap<ReactionAddDto, Reaction>();
        CreateMap<Reaction, ReactionGetDto>();
    }
}
