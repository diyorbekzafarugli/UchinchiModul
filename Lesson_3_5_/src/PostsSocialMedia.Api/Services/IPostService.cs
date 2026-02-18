using PostsSocialMedia.Api.Dtos.PostDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface IPostService
{
    Result<Guid> Create(PostCreateDto postCreateDto);
    Result<PostGetDto> GetById(Guid id, Guid userId);
    Result<List<PostGetDto>> GetByUserName(string userName, Guid userId);
    Result<bool> Update(Guid currentUserId, PostUpdateDto postUpdateDto);
    Result<List<PostGetDto>> Search(string searchTerm, int page, int pageSize, Guid userId);
    Result<bool> Delete(Guid id, Guid userId);
}