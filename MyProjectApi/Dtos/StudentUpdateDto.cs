namespace MyProject.Dtos;

public class StudentUpdateDto
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public int Age { get; set; }
}
