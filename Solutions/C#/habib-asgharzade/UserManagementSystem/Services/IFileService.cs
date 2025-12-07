namespace UserManagementSystem.Services
{
    public interface IFileService
    {
        Task<string> SaveDocumentFileAsync(byte[] fileBytes, string fileName, Guid userId);
        Task<string> SimulatePdfConversionAsync(string documentPath, Guid userId);
        Task CleanupUnusedFilesAsync();
    }
}