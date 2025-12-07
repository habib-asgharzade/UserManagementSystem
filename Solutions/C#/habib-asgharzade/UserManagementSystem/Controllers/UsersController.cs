using Hangfire;
using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.DTOs;
using UserManagementSystem.Jobs;
using UserManagementSystem.Models;
using UserManagementSystem.Services;

namespace UserManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UsersController(IUserService userService, IBackgroundJobClient backgroundJobClient)
        {
            _userService = userService;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterUserDto requestDto)
        {
            // Save user with document bytes in database
            User user = await _userService.RegisterUserAsync(requestDto);

            // Send welcome message immediately
            _backgroundJobClient.Enqueue<WelcomeMessageJob>(
                                    x => x.SendWelcomeMessageAsync(user.Id, user.Name));

            // Schedule document upload for 30 seconds later
            _backgroundJobClient.Schedule<UploadDocumentJob>(
                                    x => x.UploadAndProcessDocumentAsync(user.Id, requestDto.Document.FileName),
                                    TimeSpan.FromSeconds(30));

            return Ok(new RegisterUserResponseDto
            {
                UserId = user.Id,
                Status = "Registered",
                Message = "User registered successfully. Wellcome to the Dotin user management system. Document will be uploaded in 30 seconds."
            });
        }        
    }
}