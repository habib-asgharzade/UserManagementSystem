using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.DTOs;
using UserManagementSystem.Models;

namespace UserManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterUserAsync(RegisterUserDto requestDto)
        {
            // Check if email exists
            if (await _context.Users.AnyAsync(u => u.Email == requestDto.Email))
            {
                throw new InvalidOperationException("Email already registered");
            }

            // Read file into byte array
            using var memoryStream = new MemoryStream();
            await requestDto.Document.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var user = new User
            {
                Name = requestDto.Name,
                Email = requestDto.Email,
                DocumentFileName = requestDto.Document.FileName,
                DocumentBytes = fileBytes,  // Store file temporarily in DB
                DocumentStatus = "Pending Upload",
                RegisteredAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task UpdateDocumentStatusAsync(Guid userId, string status, string documentPath = null, string pdfPath = null)
        {
            var user = await GetUserAsync(userId);

            if (user != null)
            {
                user.DocumentStatus = status;

                if (!string.IsNullOrEmpty(documentPath))
                    user.DocumentPath = documentPath;

                if (!string.IsNullOrEmpty(pdfPath))
                    user.PdfPath = pdfPath;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<byte[]?> GetUserDocumentBytesAsync(Guid userId)
        {
            byte[]? user = await _context.Users
                                            .Where(u => u.Id == userId)
                                            .Select(u => u.DocumentBytes)
                                            .FirstOrDefaultAsync();

            return user;
        }

        public async Task ClearDocumentBytesAsync(Guid userId)
        {
            var user = await GetUserAsync(userId);
            if (user != null)
            {
                user.DocumentBytes = null;  // Clear after upload
                await _context.SaveChangesAsync();
            }
        }
    }
}