using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCoursePlatform.Application.DTOs.Courses.Requestes;
using StudentCoursePlatform.Application.Interfaces;

namespace StudentCoursePlatform.Api.Controllers;

[ApiController]
[Route("api/cources")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateCourseAsync([FromBody] CreateCourseDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _courseService.CreateAsync(dto, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpGet("by-techer")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetByTeacherIdAsync(CancellationToken cancellationToken)
    {
        var result = await _courseService.GetByTeacherIdAsync(cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateCourseDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _courseService.UpdateAsync(id, dto, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _courseService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}publish")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> PublishAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _courseService.PublishAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}unpublish")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UnpublishAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _courseService.UnpublishAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _courseService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }
}
