using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StudentCoursePlatform.Application.Interfaces;

namespace StudentCoursePlatform.Infrastructure.Services;

public class CloudinaryFileService : IFileService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryFileService(IConfiguration config)
    {
        var account = new Account(
            config["CloudinarySettings:CloudName"],
            config["CloudinarySettings:ApiKey"],
            config["CloudinarySettings:ApiSecret"]
        );

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Fayl bo'sh bo'lishi mumkin emas.");

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"StudentCoursePlatform/{folderName}",
            Transformation = new Transformation().Quality("auto").FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception($"Cloudinary xatoligi: {uploadResult.Error.Message}");

        // Yuklangan faylning to'liq manzilini qaytaramiz (https://res.cloudinary.com/...)
        return uploadResult.SecureUrl.ToString();
    }

    public bool DeleteFile(string fileUrl)
    {
        return true;
    }
}