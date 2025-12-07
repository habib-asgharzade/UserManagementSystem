using Microsoft.Extensions.Configuration;

namespace UserManagementSystem.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> SaveDocumentFileAsync(byte[] fileBytes, string fileName, Guid userId)
        {
            var uploadPath = Path.Combine("Uploads", userId.ToString());
            Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            return filePath;
        }

        public async Task<string> SimulatePdfConversionAsync(string documentPath, Guid userId)
        {
            await Task.Delay(2000); // Simulate conversion time

            var pdfPath = Path.Combine("PDFs", userId.ToString());
            Directory.CreateDirectory(pdfPath);

            var pdfFileName = $"{Path.GetFileNameWithoutExtension(documentPath)}.pdf";
            var pdfFilePath = Path.Combine(pdfPath, pdfFileName);

            // Create dummy PDF file
            await File.WriteAllTextAsync(pdfFilePath, "This is a simulated PDF file");

            return pdfFilePath;
        }

        public Task CleanupUnusedFilesAsync()
        {
            CleanupDirectory("Uploads", 7);
            CleanupDirectory("PDFs", 7);
            return Task.CompletedTask;
        }

        private void CleanupDirectory(string directory, int days)
        {
            if (!Directory.Exists(directory)) return;

            var cutoff = DateTime.Now.AddDays(-days);

            foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                if (File.GetLastWriteTime(file) < cutoff)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { /* Ignore */ }
                }
            }
        }
    }
}