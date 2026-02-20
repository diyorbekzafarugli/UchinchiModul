using PostsSocialMedia.Api.Dtos.CommentDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface ICommentService
{
    Task<Result<Guid>> Add(Guid currentUserId, CommentAddDto commentDto);

    Task<Result<CommentGetDto>> GetById(Guid currentUserId, Guid id);

    Task<Result<List<CommentGetDto>>> GetAll(Guid currentUserId);

    Task<Result<bool>> Delete(Guid currentUserId, Guid id);

    Task<Result<List<CommentGetDto>>> GetByPostId(Guid currentUserId, Guid postId);

    Task<Result<List<CommentGetDto>>> GetUserCommentsInPost(Guid currentUserId, Guid postId);
}