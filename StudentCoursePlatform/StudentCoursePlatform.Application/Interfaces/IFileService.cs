using Microsoft.AspNetCore.Http;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName,
        CancellationToken cancellationToken);

    bool DeleteFile(string fileUrl);
}
