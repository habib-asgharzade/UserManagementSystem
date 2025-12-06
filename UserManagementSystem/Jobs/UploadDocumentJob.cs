using Hangfire;
using Microsoft.Extensions.Logging;
using UserManagementSystem.DTOs;
using UserManagementSystem.Services;

namespace UserManagementSystem.Jobs
{
    public class UploadDocumentJob
    {
        private readonly ILogger<UploadDocumentJob> _logger;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UploadDocumentJob(
            ILogger<UploadDocumentJob> logger,
            IFileService fileService,
            IUserService userService,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _fileService = fileService;
            _userService = userService;
            _backgroundJobClient = backgroundJobClient;
        }

        [AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 300, 600 })]
        public async Task UploadAndProcessDocumentAsync(Guid userId, string fileName)
        {
            try
            {
                _logger.LogInformation($"Starting document upload for user {userId}");
                                
                // Get file bytes from database
                var fileBytes = await _userService.GetUserDocumentBytesAsync(userId);
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    throw new Exception($"No document found for user {userId}");
                }

                // Update status to Uploading
                await _userService.UpdateDocumentStatusAsync(userId, "Uploading");

                // Save file to disk
                var documentPath = await _fileService.SaveDocumentFileAsync(fileBytes, fileName, userId);

                // Update status to Uploaded
                await _userService.UpdateDocumentStatusAsync(userId, "Uploaded", documentPath);

                // Clear bytes from database to save space
                await _userService.ClearDocumentBytesAsync(userId);

                _logger.LogInformation($"Document uploaded for user {userId}");

                // Now schedule PDF conversion
                _backgroundJobClient.Enqueue<DocumentProcessingJob>(
                                            x => x.ProcessDocumentAsync(userId, documentPath));
            }
            catch (Exception ex)
            {
                await _userService.UpdateDocumentStatusAsync(userId, "Upload Failed");
                _logger.LogError(ex, $"Error uploading document for user {userId}");
                throw;
            }
        }
    }
}