namespace MessagingApp.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>Saves a profile picture for the given user, replacing any existing one, and returns its public path.</summary>
    Task<string> SaveProfilePictureAsync(Guid userId, Stream content, string fileExtension);
}
