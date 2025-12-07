using UserManagementSystem.DTOs;
using UserManagementSystem.Models;

namespace UserManagementSystem.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(RegisterUserDto dto);
        Task<User> GetUserAsync(Guid userId);
        Task UpdateDocumentStatusAsync(Guid userId, string status, string? documentPath = null, string? pdfPath = null);
        Task<byte[]> GetUserDocumentBytesAsync(Guid userId);
        Task ClearDocumentBytesAsync(Guid userId);
    }
}