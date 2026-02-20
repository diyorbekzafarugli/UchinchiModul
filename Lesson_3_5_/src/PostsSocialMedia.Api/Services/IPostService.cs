using PostsSocialMedia.Api.Dtos.PostDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface IPostService
{
    Task<Result<Guid>> Create(PostCreateDto postCreateDto);

    Task<Result<PostGetDto>> GetById(Guid id, Guid userId);

    Task<Result<List<PostGetDto>>> GetByUserName(string userName, Guid userId);

    Task<Result<bool>> Update(Guid currentUserId, PostUpdateDto postUpdateDto);

    Task<Result<List<PostGetDto>>> Search(string searchTerm, int page, int pageSize, Guid userId);

    Task<Result<bool>> Delete(Guid id, Guid userId);
}