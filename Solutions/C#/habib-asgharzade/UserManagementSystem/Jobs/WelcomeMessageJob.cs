using Hangfire;
using Microsoft.Extensions.Logging;
using UserManagementSystem.Data;
using UserManagementSystem.Models;

namespace UserManagementSystem.Jobs
{
    public class WelcomeMessageJob
    {
        private readonly ILogger<WelcomeMessageJob> _logger;
        private readonly ApplicationDbContext _context;

        public WelcomeMessageJob(
            ILogger<WelcomeMessageJob> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 300, 600 })] // 5 min, 10 min
        public async Task SendWelcomeMessageAsync(Guid userId, string userName)
        {
            try
            {
                _logger.LogInformation($"Sending welcome message to {userName} ({userId})");

                // Simulate sending welcome message (email, notification, etc.)
                await Task.Delay(1000); // Simulate work

                // Log the job completion
                var jobLog = new JobLog
                {
                    JobId = Guid.NewGuid().ToString(),
                    JobType = "WelcomeMessage",
                    Status = "Completed",
                    Message = $"Welcome message sent to {userName}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.JobLogs.Add(jobLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Welcome message sent to {userName}");
            }
            catch (Exception ex)
            {
                // Log the failure
                var jobLog = new JobLog
                {
                    JobId = Guid.NewGuid().ToString(),
                    JobType = "WelcomeMessage",
                    Status = "Failed",
                    Message = $"Failed to send welcome message: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.JobLogs.Add(jobLog);
                await _context.SaveChangesAsync();

                _logger.LogError(ex, $"Error sending welcome message to {userName}");
                throw; // Let Hangfire handle the retry
            }
        }
    }
}