using PostsSocialMedia.Api.Dtos.CommentDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface ICommentService
{
    Result<Guid> Add(Guid currentUserId, CommentAddDto commentDto);
    Result<CommentGetDto>? GetById(Guid currentUserId, Guid id);
    Result<List<CommentGetDto>> GetAll(Guid currentUserId);
    Result<bool> Delete(Guid currentUserId, Guid id);
    Result<List<CommentGetDto>> GetByPostId(Guid currentUserId, Guid postId);
    Result<List<CommentGetDto>> GetUserCommentsInPost(Guid currentUserId, Guid postId);
}
