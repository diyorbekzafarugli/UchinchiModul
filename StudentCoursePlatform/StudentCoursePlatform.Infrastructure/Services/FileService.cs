using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StudentCoursePlatform.Application.Interfaces;

namespace StudentCoursePlatform.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }
    public async Task<string> UploadFileAsync(IFormFile file, string folderName,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Fayl bo'sh bo'lishi mumkin emas.");

        var uploadsFolder = Path.Combine(_env.WebRootPath ??
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", folderName);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        return $"/uploads/{folderName}/{uniqueFileName}";
    }

    public bool DeleteFile(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return false;

        var filePath = Path.Combine(_env.WebRootPath ??
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), fileUrl.TrimStart('/'));

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }

        return false;
    }
}
