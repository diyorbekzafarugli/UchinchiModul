using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCoursePlatform.Application.DTOs.Quizzes.Requests;
using StudentCoursePlatform.Application.Interfaces;

namespace StudentCoursePlatform.Api.Controllers;

[ApiController]
[Route("api/quizzes")]
[Authorize]
public class QuizzesController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizzesController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateQuizDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _quizService.CreateAsync(dto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpGet("course/{courseId:guid}")]
    public async Task<IActionResult> GetByCourseIdAsync([FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        var result = await _quizService.GetByCourseIdAsync(courseId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }
}