using MyProject.Dtos;
using MyProject.Entities;
using MyProject.Interfaces;

namespace MyProject.Services;

public class StudentService : IStudentService
{
    private readonly IRepository<Student> _repository;
    public StudentService(IRepository<Student> repository)
    {
        _repository = repository;
    }
    public Guid? Add(StudentCreateDto studentDto)
    {
        if (string.IsNullOrWhiteSpace(studentDto.FullName) || studentDto.Age < 0) return null;

        Student student = new Student()
        {
            FullName = studentDto.FullName,
            Age = studentDto.Age
        };
        return _repository.Add(student);
    }

    public bool Delete(Guid id)
    {
        return _repository.Delete(id);
    }

    public List<StudentResultDto> GetAll()
    {
        var students = _repository.GetAll();
        if (students.Count == 0) return new List<StudentResultDto>();

        var resultDto = new List<StudentResultDto>();

        foreach (var student in students)
        {
            var studentDto = new StudentResultDto()
            {
                FullName = student.FullName,
                Age = student.Age,
                Id = student.Id
            };
            resultDto.Add(studentDto);
        }
        return resultDto;
    }

    public StudentResultDto? GetById(Guid id)
    {
        var student = _repository.GetById(id);
        if (student is null) return null;

        var resultDto = new StudentResultDto()
        {
            FullName = student.FullName,
            Age = student.Age,
            Id = student.Id
        };
        return resultDto;
    }

    public bool Update(StudentUpdateDto studentUpdateDto)
    {
        if (string.IsNullOrWhiteSpace(studentUpdateDto.FullName) || studentUpdateDto.Age < 0) return false;
        var student = new Student()
        {
            FullName = studentUpdateDto.FullName,
            Age = studentUpdateDto.Age,
            Id = studentUpdateDto.Id
        };
        return _repository.Update(student);
    }
}
