using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCoursePlatform.Application.DTOs.HomeworkSubmissions.Requests;
using StudentCoursePlatform.Application.Interfaces;

namespace StudentCoursePlatform.Api.Controllers;

[ApiController]
[Route("api/homework-submissions")]
[Authorize]
public class HomeworkSubmissionsController : ControllerBase
{
    private readonly IHomeworkSubmissionService _submissionService;

    public HomeworkSubmissionsController(IHomeworkSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SubmitAsync([FromBody] SubmitHomeworkDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _submissionService.SubmitHomeworkAsync(dto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}/grade")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GradeAsync([FromRoute] Guid id,
        [FromBody] GradeSubmissionDto dto, CancellationToken cancellationToken)
    {
        var result = await _submissionService.GradeSubmissionAsync(id, dto,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }
}