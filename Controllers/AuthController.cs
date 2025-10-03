using ExpenseTracker.DTOs.AuthDtos;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> LoginAsync(LoginRequestDto loginRequest)
        {     
                var response = await _authService.LoginAsync(loginRequest);

                if (response is null)
                {
                    _logger.LogWarning("Invalid login attempt for user {UserName}", loginRequest.Username);
                    return Unauthorized(new ApiResponse<LoginResponseDto>("Invalid login"));
                }

                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful"));            
        }
    }
}
