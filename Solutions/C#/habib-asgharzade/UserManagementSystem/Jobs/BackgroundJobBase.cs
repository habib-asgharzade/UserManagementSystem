using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagementSystem.Data;
using UserManagementSystem.Models;

namespace UserManagementSystem.Jobs
{
    /*public abstract class BackgroundJobBase
    {
        protected readonly ILogger _logger;
        protected readonly ApplicationDbContext _context;
        protected readonly IBackgroundJobClient _backgroundJobClient;

        protected BackgroundJobBase(ILogger logger, ApplicationDbContext context, IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _context = context;
            _backgroundJobClient = backgroundJobClient;
        }

        protected async Task LogJobAsync(string jobId, string jobType, string status, string message, int retryCount = 0)
        {
            var jobLog = new JobLog
            {
                JobId = jobId ?? Guid.NewGuid().ToString(),
                JobType = jobType,
                Status = status,
                Message = message,
                RetryCount = retryCount
            };

            _context.JobLogs.Add(jobLog);
            await _context.SaveChangesAsync();
        }

        protected async Task HandleJobFailure(Exception ex, string jobId, string jobType, string jobMethod, params object[] args)
        {
            _logger.LogError(ex, $"Job {jobId} ({jobType}) failed");

            // Log the failure
            await LogJobAsync(jobId, jobType, "Failed",
                $"Job failed: {ex.Message}. Retry will be attempted.");

            // Check retry count from database
            var retryCount = await GetRetryCountForJob(jobId);

            if (retryCount < 2) // Maximum 2 retries
            {
                var delay = retryCount == 0 ? TimeSpan.FromMinutes(5) : TimeSpan.FromMinutes(10);
                _logger.LogInformation($"Scheduling retry {retryCount + 1} for job {jobId} after {delay.TotalMinutes} minutes");

                // Schedule retry
                _backgroundJobClient.Schedule(() => RetryJobAsync(jobId, jobType, jobMethod, args), delay);
            }
            else
            {
                _logger.LogError($"Job {jobId} failed after maximum retries");
                await LogJobAsync(jobId, jobType, "Exhausted",
                    $"Job failed after maximum retries: {ex.Message}");
            }
        }

        private async Task<int> GetRetryCountForJob(string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
                return 0;

            var jobLogs = await _context.JobLogs
                .Where(j => j.JobId == jobId && j.Status == "Failed")
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return jobLogs.Count;
        }

        protected abstract Task RetryJobAsync(string jobId, string jobType, string jobMethod, object[] args);
    }*/
}