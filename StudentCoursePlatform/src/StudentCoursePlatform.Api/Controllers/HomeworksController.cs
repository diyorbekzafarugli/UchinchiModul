using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Homeworks.Requests;
using StudentCoursePlatform.Application.Interfaces;

namespace StudentCoursePlatform.Api.Controllers;

[ApiController]
[Route("api/homeworks")]
[Authorize]
public class HomeworksController : ControllerBase
{
    private readonly IHomeworkService _homeworkService;

    public HomeworksController(IHomeworkService homeworkService)
    {
        _homeworkService = homeworkService;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateHomeworkDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _homeworkService.CreateAsync(dto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpGet("course/{courseId:guid}")]
    public async Task<IActionResult> GetByCourseIdAsync([FromRoute] Guid courseId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParams(page, pageSize);
        var result = await _homeworkService.GetByCourseIdAsync(courseId, 
            pagination, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }
}