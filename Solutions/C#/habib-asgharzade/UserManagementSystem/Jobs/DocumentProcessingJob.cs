using Hangfire;
using Microsoft.Extensions.Logging;
using UserManagementSystem.Services;

namespace UserManagementSystem.Jobs
{
    public class DocumentProcessingJob
    {
        private readonly ILogger<DocumentProcessingJob> _logger;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public DocumentProcessingJob(
            ILogger<DocumentProcessingJob> logger,
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
        public async Task ProcessDocumentAsync(Guid userId, string documentPath)
        {
            try
            {
                _logger.LogInformation($"Processing document for user {userId}");

                // Update status to Processing
                await _userService.UpdateDocumentStatusAsync(userId, "Processing");

                // Convert to PDF (simulated)
                var pdfPath = await _fileService.SimulatePdfConversionAsync(documentPath, userId);

                // Update status to Completed
                await _userService.UpdateDocumentStatusAsync(userId, "Completed", pdfPath: pdfPath);

                _logger.LogInformation($"Document processed for user {userId}");

                // Send completion message
                _backgroundJobClient.Enqueue<CompletionMessageJob>(
                    x => x.SendCompletionMessageAsync(userId));
            }
            catch (Exception ex)
            {
                await _userService.UpdateDocumentStatusAsync(userId, "Processing Failed");
                _logger.LogError(ex, $"Error processing document for user {userId}");
                throw;
            }
        }
    }
}