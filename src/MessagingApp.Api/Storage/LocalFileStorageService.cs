using MessagingApp.Application.Interfaces;

namespace MessagingApp.Api.Storage;

/// <summary>
/// Stores profile pictures on local disk under wwwroot, served back out via static files.
/// A real production deployment would likely swap this for cloud object storage (e.g. S3/Blob)
/// behind the same IFileStorageService contract, without touching any calling code.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveProfilePictureAsync(Guid userId, Stream content, string fileExtension)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profile-pictures");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{userId:N}{fileExtension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await content.CopyToAsync(fileStream);

        return $"/uploads/profile-pictures/{fileName}";
    }
}
