using Microsoft.AspNetCore.Mvc;
using MyProject.Dtos;
using MyProject.Interfaces;

namespace MyProjectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    // GET: api/students
    [HttpGet]
    public IActionResult GetAll()
    {
        var students = _service.GetAll();
        return Ok(ApiResponse<List<StudentResultDto>>.Ok(students));
    }

    // GET: api/students/{id}
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var student = _service.GetById(id);

        if (student is null)
            return NotFound("Student topilmadi");

        return Ok(student);
    }

    // POST: api/students
    [HttpPost]
    public IActionResult Create(StudentCreateDto dto)
    {
        var id = _service.Add(dto);
        if (id is null)
            return BadRequest(ApiResponse<Guid>.Fail("Yaratishda xatolik"));

        return Ok(ApiResponse<Guid>.Ok(id.Value, "Student yaratildi"));
    }

    // PUT: api/students
    [HttpPut]
    public IActionResult Update(StudentUpdateDto dto)
    {
        var updated = _service.Update(dto);

        if (!updated)
            return BadRequest("Update xato (Id yoki ma'lumot noto‘g‘ri)");

        return Ok(updated);
    }

    // DELETE: api/students/{id}
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var deleted = _service.Delete(id);

        if (!deleted)
            return NotFound("Student topilmadi");

        return Ok(deleted);
    }
}
