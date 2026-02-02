using MyProject.Dtos;

namespace MyProject.Interfaces;

public interface IStudentService
{
    Guid? Add(StudentCreateDto studentDto);
    List<StudentResultDto> GetAll();
    StudentResultDto? GetById(Guid id);
    bool Update(StudentUpdateDto studentUpdateDto);
    bool Delete(Guid id);
}
