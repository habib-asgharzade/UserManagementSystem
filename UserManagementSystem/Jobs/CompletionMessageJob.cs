using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagementSystem.Data;
using UserManagementSystem.Models;

namespace UserManagementSystem.Jobs
{
    public class CompletionMessageJob
    {
        private readonly ILogger<CompletionMessageJob> _logger;
        private readonly ApplicationDbContext _context;

        public CompletionMessageJob(
            ILogger<CompletionMessageJob> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 300, 600 })]
        public async Task SendCompletionMessageAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new Exception($"User {userId} not found");
                }

                _logger.LogInformation($"Sending completion message to {user.Name} ({user.Email})");

                // Simulate sending completion message
                await Task.Delay(1000);

                // Log the job completion
                var jobLog = new JobLog
                {
                    JobId = Guid.NewGuid().ToString(),
                    JobType = "CompletionMessage",
                    Status = "Completed",
                    Message = $"Completion message sent to {user.Name}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.JobLogs.Add(jobLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Completion message sent to {user.Name}");
            }
            catch (Exception ex)
            {
                // Log the failure
                var jobLog = new JobLog
                {
                    JobId = Guid.NewGuid().ToString(),
                    JobType = "CompletionMessage",
                    Status = "Failed",
                    Message = $"Failed to send completion message: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.JobLogs.Add(jobLog);
                await _context.SaveChangesAsync();

                _logger.LogError(ex, $"Error sending completion message to user {userId}");
                throw;
            }
        }
    }
}