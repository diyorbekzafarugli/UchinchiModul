namespace MyProject.Dtos;

public class StudentResultDto
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public int Age { get; set; }
}
