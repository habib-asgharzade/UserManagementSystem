using System;
using System.ComponentModel.DataAnnotations;


namespace UserManagementSystem.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime? RegisteredAt { get; set; } = DateTime.Now;

        public string? DocumentPath { get; set; }
        public string? DocumentStatus { get; set; } = "Pending";
        public string? PdfPath { get; set; }
        public byte[]? DocumentBytes { get; set; }  // Store file in DB temporarily
        public string? DocumentFileName { get; set; }
    }
}
