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

            try
            {
                var response = await _authService.LoginAsync(loginRequest);

                if (response is null)
                {
                    _logger.LogWarning("Invalid login attempt for user {UserName}", loginRequest.Username);
                    return Unauthorized(new ApiResponse<LoginResponseDto>("Invalid login"));
                }

                return Ok(new ApiResponse<LoginResponseDto>(response));

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access: {Message}", ex.Message);
                return BadRequest (new ApiResponse<LoginResponseDto>(ex.Message));
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred during login for user {UserName}", loginRequest.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<LoginResponseDto>("Database error occurred"));
            }

        }
    }
}
