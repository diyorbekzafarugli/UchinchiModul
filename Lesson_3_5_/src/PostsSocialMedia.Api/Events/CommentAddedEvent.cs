using MediatR;

namespace PostsSocialMedia.Api.Events;

public record CommentAddedEvent(
    Guid RecipientId,
    string CommenterName,
    string Message,
    bool IsReply
) : INotification;