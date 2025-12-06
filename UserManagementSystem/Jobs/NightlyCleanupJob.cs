using Hangfire;
using Microsoft.Extensions.Logging;
using UserManagementSystem.Data;
using UserManagementSystem.Models;
using UserManagementSystem.Services;

namespace UserManagementSystem.Jobs
{
    public class NightlyCleanupJob
    {
        private readonly ILogger<NightlyCleanupJob> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public NightlyCleanupJob(
            ILogger<NightlyCleanupJob> logger,
            ApplicationDbContext context,
            IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        [AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 300, 600 })]
        public async Task ExecuteAsync()
        {
            try
            {
                _logger.LogInformation("Starting nightly cleanup job");

                // Log job start
                var jobLog = new JobLog
                {
                    JobId = Guid.NewGuid().ToString(),
                    JobType = "NightlyCleanup",
                    Status = "Started",
                    Message = "Starting nightly cleanup",
                    CreatedAt = DateTime.UtcNow
                };
                _context.JobLogs.Add(jobLog);
                await _context.SaveChangesAsync();

                await _fileService.CleanupUnusedFilesAsync();

                // Also clean up old job logs (older than 30 days)
                var cutoffDate = DateTime.UtcNow.AddDays(-30);
                var oldLogs = _context.JobLogs
                    .Where(j => j.CreatedAt < cutoffDate);

                _context.JobLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();

                // Log job completion
                jobLog = new JobLog
                {
                    JobId = Guid.NewGuid().ToString(),
                    JobType = "NightlyCleanup",
                    Status = "Completed",
                    Message = "Nightly cleanup completed successfully",
                    CreatedAt = DateTime.UtcNow
                };
                _context.JobLogs.Add(jobLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Nightly cleanup job completed");
            }
            catch (Exception ex)
            {
                // Log failure
                var jobLog = new JobLog
                {
                    JobId = Guid.NewGuid().ToString(),
                    JobType = "NightlyCleanup",
                    Status = "Failed",
                    Message = $"Nightly cleanup failed: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.JobLogs.Add(jobLog);
                await _context.SaveChangesAsync();

                _logger.LogError(ex, "Error during nightly cleanup");
                throw;
            }
        }
    }
}