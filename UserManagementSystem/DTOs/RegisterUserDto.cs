using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public IFormFile Document { get; set; }
    }
}

public class RegisterUserResponseDto
{
    public Guid UserId { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}