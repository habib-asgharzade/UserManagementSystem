using System;

namespace UserManagementSystem.Models
{
    public class JobLog
    {
        public int Id { get; set; }
        public string JobId { get; set; }
        public string JobType { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int RetryCount { get; set; } = 0;
    }

}
