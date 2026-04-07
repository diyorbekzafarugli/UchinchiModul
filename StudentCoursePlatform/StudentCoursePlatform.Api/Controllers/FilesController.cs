using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCoursePlatform.Application.Interfaces;

namespace StudentCoursePlatform.Api.Controllers;

[ApiController]
[Route("api/files")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file,
        CancellationToken cancellationToken)
    {
        if (!file.ContentType.StartsWith("image/"))
            return BadRequest(new { isSuccess = false, errors = new[] { "Faqat rasm yuklash mumkin!" } });

        var fileUrl = await _fileService.UploadFileAsync(file, "images", cancellationToken);
        return Ok(new { isSuccess = true, url = fileUrl });
    }

    [HttpPost("upload-document")]
    public async Task<IActionResult> UploadDocument(IFormFile file,
        CancellationToken cancellationToken)
    {
        var fileUrl = await _fileService.UploadFileAsync(file, "documents", cancellationToken);
        return Ok(new { isSuccess = true, url = fileUrl });
    }
}