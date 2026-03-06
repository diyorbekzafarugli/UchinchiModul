namespace StudentCoursePlatform.Domain.Entities;

public class AnswerOption : IEntity
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    public Question Question { get; set; } = null!;
}