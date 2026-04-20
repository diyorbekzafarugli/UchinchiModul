namespace StudentCoursePlatform.Domain.Entities;

public class Question : IEntity
{
    public Guid Id { get; set; }

    public Guid QuizId { get; set; }

    public string Text { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Order { get; set; }

    public Quiz Quiz { get; set; } = null!;
    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}